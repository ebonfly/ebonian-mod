using System;
using ReLogic.Graphics;

namespace EbonianMod.Common.UI.Titledrop;

public class VoidTitledrop : TitledropStyle, ILoadable
{
	public static VoidTitledrop Instance;
	public void Load(Mod mod) => Instance = new VoidTitledrop();
	public void Unload() => Instance = null;

	readonly string title = "Warden of the Unending Abyss";
	readonly string name = "Ceaseless Void";
	
	private int time;
	private int highlightedChar;
	private int highlightedCharName;
	private string filledText;
	private string filledTextName;
	private float nameAlpha;
	private float glowAlpha;
	public override void Activate()
	{
		time = 0;
		nameAlpha = 0;
		glowAlpha = 0;
		highlightedChar = -1;
		highlightedCharName = -1;
		filledText = "";
		filledTextName = "";
	}

	public override void Update()
	{
		time++;

		if (highlightedChar <= title.Length && time % 5 == 0 && time > 20)
		{
			highlightedChar++;
			filledText = title[..(highlightedChar + 1)];
		}

		if (time == 175)
		{
			nameAlpha = 1;
			glowAlpha = 1;
		}
		
		glowAlpha = Lerp(glowAlpha, 0, 0.04f);
		
		if (time >= 270)
			TitledropSystem.Instance.Active = false;
	}

	public override void Draw()
	{
		float progress = Utils.GetLerpValue(0, 270, time);
		float alpha = Clamp(MathF.Sin(progress * MathF.PI) * 3, 0, 1);
		Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * alpha);
		
		DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.MouseText.Value, filledText, new Vector2(Main.screenWidth / 2f - FontAssets.MouseText.Value.MeasureString(filledText).X / 2f, Main.screenHeight * 0.4f), Color.White * alpha * alpha);
		DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.MouseText.Value, filledText, new Vector2(Main.screenWidth / 2f - FontAssets.MouseText.Value.MeasureString(filledText).X / 2f, Main.screenHeight * 0.4f), Color.DarkViolet with { A = 0 }* alpha);
		DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, name, new Vector2(Main.screenWidth / 2f - FontAssets.DeathText.Value.MeasureString(name).X / 2f, Main.screenHeight * 0.45f), Color.White * nameAlpha * alpha * alpha);
		DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, name, new Vector2(Main.screenWidth / 2f - FontAssets.DeathText.Value.MeasureString(name).X / 2f, Main.screenHeight * 0.45f), Color.DarkViolet with { A = 0 } * nameAlpha* alpha);
		
		Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * glowAlpha);
		
	}
}