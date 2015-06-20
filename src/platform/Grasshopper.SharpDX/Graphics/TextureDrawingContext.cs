﻿using Grasshopper.Graphics.Rendering;
using Grasshopper.SharpDX.Graphics.Rendering;
using Grasshopper.SharpDX.Graphics.Rendering.Internal;
using SharpDX.Direct3D11;
using Color = Grasshopper.Graphics.Primitives.Color;
using Color4 = SharpDX.Color4;

namespace Grasshopper.SharpDX.Graphics
{
	class TextureDrawingContext : DrawingContext, ITextureDrawingContext
	{
		private readonly TextureBuffer _textureBuffer;
		private readonly DepthBuffer _depthBuffer;
		private bool _isInitialized;

		public TextureDrawingContext(DeviceManager deviceManager, int width, int height)
			: base(deviceManager: deviceManager)
		{
			Width = width;
			Height = height;

			_textureBuffer = new TextureBuffer(deviceManager, width, height);
			_depthBuffer = new DepthBuffer(deviceManager, width, height);
		}

		public int Width { get; private set; }
		public int Height { get; private set; }

		public void Initialize()
		{
			if(_isInitialized)
				DestroyResources();

			_textureBuffer.Initialize();
			_depthBuffer.Initialize();
			_isInitialized = true;
		}

		protected override void DestroyResources()
		{
			_textureBuffer.Dispose();
			_depthBuffer.Dispose();
			_isInitialized = false;
		}

		protected override void MakeTargetsActive()
		{
			DeviceManager.Context.OutputMerger.SetTargets(_depthBuffer.DepthStencilView, _textureBuffer.RenderTargetView);
		}

		public override void Clear(Color color)
		{
			if(_depthBuffer.DepthStencilView != null)
				DeviceManager.Context.ClearDepthStencilView(_depthBuffer.DepthStencilView, DepthStencilClearFlags.Depth, 1f, 0);

			if(_textureBuffer.RenderTargetView != null)
				DeviceManager.Context.ClearRenderTargetView(_textureBuffer.RenderTargetView, new Color4(color.ToRgba()));
		}
	}
}