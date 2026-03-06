using System;
using System.Collections.Generic;

namespace EbonianMod.Common.Graphics.RenderTargets;

public abstract class BaseCachedActionRenderTarget<T> : ModSystem where T : BaseCachedActionRenderTarget<T> 
{
	public virtual int TargetAmount => 1;
	public RenderTarget2D[] Targets;
	internal List<Action> _drawCache = [];
	public static List<Action> DrawCache => ModContent.GetInstance<T>()._drawCache;

	public override void Load()
	{
		Targets = new RenderTarget2D[TargetAmount];
		_drawCache ??= [];
		
		void InitRender()
		{
			if (Main.netMode != NetmodeID.Server)
				Main.QueueMainThreadAction(() =>
				{
					for (int i = 0; i < TargetAmount; i++)
						RenderingUtils.CreateRender(ref Targets[i]);
				});
		}
		Main.OnResolutionChanged += (Vector2 obj) => InitRender();
		InitRender();
		
		GlobalRenderTargetRendering.PopulateTargets += PopulateTarget;
		GlobalRenderTargetRendering.DrawTargets += DrawTarget;
	}
	
	public abstract void PopulateTarget();
	public abstract void DrawTarget();
}