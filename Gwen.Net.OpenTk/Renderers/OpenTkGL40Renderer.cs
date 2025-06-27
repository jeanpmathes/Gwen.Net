using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Gwen.Net.OpenTk.Shaders;
using OpenTK.Graphics.OpenGL;
using Boolean = System.Boolean;

namespace Gwen.Net.OpenTk.Renderers
{
    public class OpenTkGL40Renderer : OpenTkRendererBase
    {
        private const Int32 MaxVerts = 4096;
        private readonly Boolean restoreRenderState;
        private readonly IShader shader;
        private readonly Int32 vertexSize;

        private readonly Vertex[] vertices;
        private Int32 prevAlphaFunc;
        private Single prevAlphaRef;
        private Int32 prevBlendDst;
        private Int32 prevBlendSrc;
        private Int32 totalVertNum;
        private Int32 vao;

        private Int32 vbo;
        private Int32 vertNum;
        
        private Boolean wasBlendEnabled;
        private Boolean wasDepthTestEnabled;

        public OpenTkGL40Renderer(IEnumerable<TexturePreload> texturePreloads, Action<TexturePreload, Exception> errorCallback, Boolean restoreRenderState = true) : base(texturePreloads, errorCallback)
        {
            vertices = new Vertex[MaxVerts];
            vertexSize = Marshal.SizeOf(vertices[0]);
            this.restoreRenderState = restoreRenderState;

            CreateBuffers();

            shader = new GL40ShaderLoader().Load("gui.gl40");
        }

        public override Int32 VertexCount => totalVertNum;

        private void CreateBuffers()
        {
            GL.GenVertexArrays(n: 1, out vao);
            GL.BindVertexArray(vao);

            GL.GenBuffers(n: 1, out vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            GL.BufferData(
                BufferTarget.ArrayBuffer,
                (IntPtr) (vertexSize * MaxVerts),
                IntPtr.Zero,
                BufferUsageHint.StreamDraw); // Allocate

            // Vertex positions
            GL.EnableVertexAttribArray(index: 0);

            GL.VertexAttribPointer(
                index: 0,
                size: 2,
                VertexAttribPointerType.Float,
                normalized: false,
                vertexSize,
                offset: 0);

            // Tex coords
            GL.EnableVertexAttribArray(index: 1);

            GL.VertexAttribPointer(
                index: 1,
                size: 2,
                VertexAttribPointerType.Float,
                normalized: false,
                vertexSize,
                2 * sizeof(Single));

            // Colors
            GL.EnableVertexAttribArray(index: 2);

            GL.VertexAttribPointer(
                index: 2,
                size: 4,
                VertexAttribPointerType.Float,
                normalized: false,
                vertexSize,
                2 * (sizeof(Single) + sizeof(Single)));

            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer: 0);
            GL.BindVertexArray(array: 0);
        }

        public override void Begin()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.UseProgram(shader.Program);

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            if (restoreRenderState)
            {
                // Get previous parameter values before changing them.
                GL.GetInteger(GetPName.BlendSrc, out prevBlendSrc);
                GL.GetInteger(GetPName.BlendDst, out prevBlendDst);
                GL.GetInteger(GetPName.AlphaTestFunc, out prevAlphaFunc);
                GL.GetFloat(GetPName.AlphaTestRef, out prevAlphaRef);

                wasBlendEnabled = GL.IsEnabled(EnableCap.Blend);
                wasDepthTestEnabled = GL.IsEnabled(EnableCap.DepthTest);
            }

            // Set default values and enable/disable caps.
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            GL.Disable(EnableCap.DepthTest);

            vertNum = 0;
            totalVertNum = 0;
            drawCallCount = 0;
            clipEnabled = false;
            textureEnabled = false;
            lastTextureID = -1;
        }

        public override void End()
        {
            Flush();
            GL.BindVertexArray(array: 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffer: 0);

            if (restoreRenderState)
            {
                GL.BindTexture(TextureTarget.Texture2D, texture: 0);
                lastTextureID = 0;

                // Restore the previous parameter values.
                GL.BlendFunc((BlendingFactor) prevBlendSrc, (BlendingFactor) prevBlendDst);

                if (!wasBlendEnabled)
                {
                    GL.Disable(EnableCap.Blend);
                }

                if (wasDepthTestEnabled)
                {
                    GL.Enable(EnableCap.DepthTest);
                }
            }
        }

        protected override void Flush()
        {
            if (vertNum == 0)
            {
                return;
            }

            if (GLVersion >= 43)
            {
                GL.InvalidateBufferData(vbo);
            }
            //else
            // This will slow down rendering. It seems to work without this but there could be synchronization problems with some drivers.
            // If that's the case then enable this.
            //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(m_VertexSize * MaxVerts), IntPtr.Zero, BufferUsageHint.StreamDraw);

            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr) (vertNum * vertexSize), vertices);

            GL.Uniform1(shader.Uniforms["uUseTexture"], textureEnabled ? 1.0f : 0.0f);

            GL.DrawArrays(PrimitiveType.Triangles, first: 0, vertNum);

            drawCallCount++;
            totalVertNum += vertNum;
            vertNum = 0;
        }

        protected override void DrawRect(Rectangle rect, Single u1 = 0, Single v1 = 0, Single u2 = 1, Single v2 = 1)
        {
            if (vertNum + 4 >= MaxVerts)
            {
                Flush();
            }

            if (clipEnabled)
            {
                // cpu scissors test

                if (rect.Y < ClipRegion.Y)
                {
                    Int32 oldHeight = rect.Height;
                    Int32 delta = ClipRegion.Y - rect.Y;
                    rect.Y = ClipRegion.Y;
                    rect.Height -= delta;

                    if (rect.Height <= 0)
                    {
                        return;
                    }

                    Single dv = delta / (Single) oldHeight;

                    v1 += dv * (v2 - v1);
                }

                if (rect.Y + rect.Height > ClipRegion.Y + ClipRegion.Height)
                {
                    Int32 oldHeight = rect.Height;
                    Int32 delta = rect.Y + rect.Height - (ClipRegion.Y + ClipRegion.Height);

                    rect.Height -= delta;

                    if (rect.Height <= 0)
                    {
                        return;
                    }

                    Single dv = delta / (Single) oldHeight;

                    v2 -= dv * (v2 - v1);
                }

                if (rect.X < ClipRegion.X)
                {
                    Int32 oldWidth = rect.Width;
                    Int32 delta = ClipRegion.X - rect.X;
                    rect.X = ClipRegion.X;
                    rect.Width -= delta;

                    if (rect.Width <= 0)
                    {
                        return;
                    }

                    Single du = delta / (Single) oldWidth;

                    u1 += du * (u2 - u1);
                }

                if (rect.X + rect.Width > ClipRegion.X + ClipRegion.Width)
                {
                    Int32 oldWidth = rect.Width;
                    Int32 delta = rect.X + rect.Width - (ClipRegion.X + ClipRegion.Width);

                    rect.Width -= delta;

                    if (rect.Width <= 0)
                    {
                        return;
                    }

                    Single du = delta / (Single) oldWidth;

                    u2 -= du * (u2 - u1);
                }
            }

            Single cR = color.R / 255f;
            Single cG = color.G / 255f;
            Single cB = color.B / 255f;
            Single cA = color.A / 255f;

            Int32 vertexIndex = vertNum;
            vertices[vertexIndex].x = (Int16) rect.X;
            vertices[vertexIndex].y = (Int16) rect.Y;
            vertices[vertexIndex].u = u1;
            vertices[vertexIndex].v = v1;
            vertices[vertexIndex].r = cR;
            vertices[vertexIndex].g = cG;
            vertices[vertexIndex].b = cB;
            vertices[vertexIndex].a = cA;

            vertexIndex++;
            vertices[vertexIndex].x = (Int16) (rect.X + rect.Width);
            vertices[vertexIndex].y = (Int16) rect.Y;
            vertices[vertexIndex].u = u2;
            vertices[vertexIndex].v = v1;
            vertices[vertexIndex].r = cR;
            vertices[vertexIndex].g = cG;
            vertices[vertexIndex].b = cB;
            vertices[vertexIndex].a = cA;

            vertexIndex++;
            vertices[vertexIndex].x = (Int16) (rect.X + rect.Width);
            vertices[vertexIndex].y = (Int16) (rect.Y + rect.Height);
            vertices[vertexIndex].u = u2;
            vertices[vertexIndex].v = v2;
            vertices[vertexIndex].r = cR;
            vertices[vertexIndex].g = cG;
            vertices[vertexIndex].b = cB;
            vertices[vertexIndex].a = cA;

            vertexIndex++;
            vertices[vertexIndex].x = (Int16) rect.X;
            vertices[vertexIndex].y = (Int16) rect.Y;
            vertices[vertexIndex].u = u1;
            vertices[vertexIndex].v = v1;
            vertices[vertexIndex].r = cR;
            vertices[vertexIndex].g = cG;
            vertices[vertexIndex].b = cB;
            vertices[vertexIndex].a = cA;

            vertexIndex++;
            vertices[vertexIndex].x = (Int16) (rect.X + rect.Width);
            vertices[vertexIndex].y = (Int16) (rect.Y + rect.Height);
            vertices[vertexIndex].u = u2;
            vertices[vertexIndex].v = v2;
            vertices[vertexIndex].r = cR;
            vertices[vertexIndex].g = cG;
            vertices[vertexIndex].b = cB;
            vertices[vertexIndex].a = cA;

            vertexIndex++;
            vertices[vertexIndex].x = (Int16) rect.X;
            vertices[vertexIndex].y = (Int16) (rect.Y + rect.Height);
            vertices[vertexIndex].u = u1;
            vertices[vertexIndex].v = v2;
            vertices[vertexIndex].r = cR;
            vertices[vertexIndex].g = cG;
            vertices[vertexIndex].b = cB;
            vertices[vertexIndex].a = cA;

            vertNum += 6;
        }

        public override void Resize(Int32 width, Int32 height)
        {
            GL.Viewport(x: 0, y: 0, width, height);
            GL.UseProgram(shader.Program);
            GL.Uniform2(shader.Uniforms["uScreenSize"], width, (Single) height);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Vertex
        {
            public Single x, y;
            public Single u, v;
            public Single r, g, b, a;
        }
    }
}
