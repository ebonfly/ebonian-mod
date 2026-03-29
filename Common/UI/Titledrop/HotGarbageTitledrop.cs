using System;
using Humanizer;
using ReLogic.Graphics;

namespace EbonianMod.Common.UI.Titledrop;

public class HotGarbageTitledrop : TitledropStyle, ILoadable
{
	public static HotGarbageTitledrop Instance;
	public void Load(Mod mod) => Instance = new HotGarbageTitledrop();
	public void Unload() => Instance = null;

	public LocalizedText Name => Language.GetText("Mods.EbonianMod.NPCs.HotGarbage.DisplayName"); 
	
	private const int MaxTime = 300;
	private int time;
	private float movementTimer;
	private Vector2 barOffset;
	private float additionalAlpha;

	public override void Activate()
	{
		time = 0;
		movementTimer = 0.5f;
		barOffset = Vector2.Zero;
		additionalAlpha = 0;
	}

	public override void Update()
	{
		time++;

		movementTimer += 0.08f * movementTimer.SafeDivision() * MathHelper.Clamp(MathF.Sin(Utils.GetLerpValue(0, 120, time) * MathF.PI), 0, 1);
		if (movementTimer > 2 && time < 120)
			movementTimer = 0;

		Vector2 destination = new Vector2(0, -60).RotatedBy(0.14f);
		if (time is > 33 and < 53)
			barOffset = Vector2.SmoothStep(Vector2.Zero, destination * 0.4f, (time - 33) / 20f);
		
		if (time is > 60 and < 70)
			barOffset = Vector2.SmoothStep(destination * 0.4f, destination, (time - 60) / 10f);
		
		if (time is > 75 and < 90)
			barOffset = Vector2.SmoothStep(destination, Vector2.Zero, MathF.Pow((time - 75) / 15f, 2));

		if (time > 80)
			additionalAlpha = MathHelper.Lerp(additionalAlpha, 1, 0.1f);
		
		if (time >= 150)
			TitledropSystem.Instance.Active = false;
	}

	public override void Draw()
	{
		float progress = Utils.GetLerpValue(0, 120, time);
		float factor = MathHelper.Clamp(MathF.Sin(progress * MathF.PI), 0, 1);
		float baseAlpha = MathHelper.Clamp(MathF.Pow(factor, 5) * 16, 0, 1);
		float textAlpha = MathHelper.Clamp(MathF.Pow(MathF.Sin(Utils.GetLerpValue(0, 150, time) * MathF.PI), 5) * 5, 0, 1);
		
		Texture2D hazard = Assets.Extras.hazardUnblurred.Value;
		Texture2D textGlow = Assets.Extras.textGlow.Value;
		Texture2D additionalGlow = Assets.Extras.laser2.Value;
		Texture2D exclamation = Assets.Extras.exclamation.Value;

		Vector2 randOffset() => Main.rand.NextVector2Circular(15, 15) * factor;

		float rotation = 0.1f;
		
        for (int k = 0;  k < 2; k++) 
        {
	        Main.spriteBatch.Draw(textGlow, new Vector2(Main.screenWidth / 2f, Main.screenHeight * 0.16f).RotatedBy(rotation), null, Color.Black * baseAlpha, rotation, textGlow.Size() / 2f, new Vector2(10, 6), SpriteEffects.None, 0);
	        Color color = k == 0 ? Color.Black : (Color.Maroon with { A = 0 });
        	
	        for (int i = -(int)(Main.screenWidth / hazard.Width); i < (int)(Main.screenWidth / hazard.Width); i++)
	        {
		        float hazardPositionX = Main.screenWidth / 2f + (i * hazard.Width);
		        float hazardOffsetX = hazard.Width * movementTimer;
		        float direction = progress > 0.5f ? 1 : -1;
		        
		        for (int j = 0; j < 2; j++)
		        {
			        float unfactoredAlpha = MathF.Pow(1 - MathF.Abs(i) / (Main.screenWidth / (float)hazard.Width), 3);
			        float unfactoredAlphaNext = MathF.Pow(1 - MathF.Abs(i + 1) / (Main.screenWidth / (float)hazard.Width), 3);
			        
			        float alpha = unfactoredAlpha * factor;
			        Main.spriteBatch.Draw(hazard, randOffset() + new Vector2(hazardPositionX + hazardOffsetX, Main.screenHeight * 0.11f).RotatedBy(rotation) + barOffset, null, color * 2 * alpha, rotation, hazard.Size() / 2f, 1, SpriteEffects.None, 0);
			        Main.spriteBatch.Draw(hazard, randOffset() + new Vector2(hazardPositionX - hazardOffsetX, Main.screenHeight * 0.25f).RotatedBy(rotation) - barOffset, null, color * 2 * alpha, rotation, hazard.Size() / 2f, 1, SpriteEffects.None, 0);
			        
			        if (k == 1) 
			        {
				        Main.spriteBatch.Draw(hazard, randOffset() * 5 + new Vector2(hazardPositionX + hazardOffsetX, Main.screenHeight * 0.11f).RotatedBy(rotation), null, color * 0.2f * alpha, rotation, hazard.Size() / 2f, 3, SpriteEffects.None, 0);
				        Main.spriteBatch.Draw(hazard, randOffset() * 5 + new Vector2(hazardPositionX - hazardOffsetX, Main.screenHeight * 0.25f).RotatedBy(rotation), null, color * 0.2f * alpha, rotation, hazard.Size() / 2f, 3, SpriteEffects.None, 0);
			        }
	
			        Vector2 exPos = randOffset() + new Vector2(Main.screenWidth / 2f + i * exclamation.Width + movementTimer * exclamation.Width * (i < 0 ? 1 : -1), Main.screenHeight * 0.178f).RotatedBy(rotation);
			        Main.spriteBatch.Draw(exclamation, exPos, null, color * 2 * alpha * Lerp(0, 2, Clamp(MathF.Abs(exPos.X - Main.screenWidth / 2f) / 5000, 0, 1)), rotation, exclamation.Size() / 2f, 0.1f, SpriteEffects.None, 0);

			        float textWidth = FontAssets.DeathText.Value.MeasureString(Name.Value).X;
			        Vector2 textPosition = randOffset() + new Vector2(Main.screenWidth / 2f - textWidth / 2 + i * textWidth * 1.5f + textWidth * movementTimer * 0.8f, Main.screenHeight * 0.15f).RotatedBy(rotation);
			        float textAlphaIndividual = Lerp(i == 0 ? 1 : 0, MathF.Pow(Lerp(unfactoredAlpha, unfactoredAlphaNext, movementTimer), 6), factor);
			        DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, Name.Value, textPosition, color * 2 * textAlpha * textAlphaIndividual, rotation, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
		        }
	        }
	        
	        Main.spriteBatch.Draw(additionalGlow, new Vector2(Main.screenWidth / 2f, Main.screenHeight * 0.176f).RotatedBy(rotation), null, Color.Maroon with { A = 0 } * baseAlpha * additionalAlpha * 2, rotation, additionalGlow.Size() / 2f, new Vector2(10, 3 * additionalAlpha), SpriteEffects.None, 0);
        }
	}
}