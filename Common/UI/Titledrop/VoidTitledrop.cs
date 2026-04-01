using System;
using ReLogic.Graphics;

namespace EbonianMod.Common.UI.Titledrop;

public class VoidTitledrop : TitledropStyle, ILoadable
{
	public static VoidTitledrop Instance;
	public void Load(Mod mod) => Instance = new VoidTitledrop();
	public void Unload() => Instance = null;

	readonly LocalizedText title = Language.GetText("Mods.EbonianMod.NPCs.Ceaseless.Title");
	readonly LocalizedText name = Language.GetText("Mods.EbonianMod.NPCs.Ceaseless.Name");
	
	private int time;
	private int highlightedChar;
	private string filledText;
	private float nameAlpha;
	private float glowAlpha;
	public override void Activate()
	{
		time = 0;
		nameAlpha = 0;
		glowAlpha = 0;
		highlightedChar = -1;
		filledText = "";
	}

	public override void Update()
	{
		time++;

		if (highlightedChar <= title.Value.Length && time % 5 == 0 && time > 20)
		{
			highlightedChar++;
			filledText = title.Value[..(highlightedChar + 1)];
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
		DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, name.Value, new Vector2(Main.screenWidth / 2f - FontAssets.DeathText.Value.MeasureString(name.Value).X / 2f, Main.screenHeight * 0.45f), Color.White * nameAlpha * alpha * alpha);
		DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, name.Value, new Vector2(Main.screenWidth / 2f - FontAssets.DeathText.Value.MeasureString(name.Value).X / 2f, Main.screenHeight * 0.45f), Color.DarkViolet with { A = 0 } * nameAlpha* alpha);
		
		Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * glowAlpha);
	}
}

public class RavagerTitledrop : TitledropStyle, ILoadable
{
	public static RavagerTitledrop Instance;
	public void Load(Mod mod) => Instance = new RavagerTitledrop();
	public void Unload() => Instance = null;
	
	readonly LocalizedText title = Language.GetText("Mods.EbonianMod.NPCs.RAVAGER.Title");
	readonly LocalizedText name = Language.GetText("Mods.EbonianMod.NPCs.RAVAGER.Name");
	
	private int time;
	private int highlightedChar;
	private string filledText;
	private float nameAlpha;
	private float glowAlpha;
	public override void Activate()
	{
		time = 0;
		nameAlpha = 0;
		glowAlpha = 0;
		highlightedChar = -1;
		filledText = "";
	}

	public override void Update()
	{
		time++;

		if (highlightedChar <= title.Value.Length && time % 5 == 0 && time > 20)
		{
			highlightedChar++;
			filledText = title.Value[..(highlightedChar + 1)];
		}

		if (time == 175)
		{
			nameAlpha = 1;
			glowAlpha = 1;
		}
		
		glowAlpha = Lerp(glowAlpha, 0, 0.04f);
		
		if (time >= 570)
			TitledropSystem.Instance.Active = false;
	}

	public override void Draw()
	{
		float progress = Utils.GetLerpValue(0, 270, time);
		float alpha = Clamp(MathF.Sin(progress * MathF.PI) * 3, 0, 1);
		if (time > 270)
			alpha = 0;
		
		progress = Utils.GetLerpValue(0, 570, time);
		float alphaText = Clamp(MathF.Sin(progress * MathF.PI) * 6, 0, 1);
		Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * alpha);
		
		DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.MouseText.Value, filledText, new Vector2(Main.screenWidth / 2f - FontAssets.MouseText.Value.MeasureString(filledText).X / 2f, Main.screenHeight * 0.4f), Color.White * alphaText * alphaText);
		DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.MouseText.Value, filledText, new Vector2(Main.screenWidth / 2f - FontAssets.MouseText.Value.MeasureString(filledText).X / 2f, Main.screenHeight * 0.4f), Main.DiscoColor with { A = 0 }* alphaText);
		DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, name.Value, new Vector2(Main.screenWidth / 2f - FontAssets.DeathText.Value.MeasureString(name.Value).X / 2f, Main.screenHeight * 0.45f), Color.White * nameAlpha * alphaText * alphaText);
		DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, name.Value, new Vector2(Main.screenWidth / 2f - FontAssets.DeathText.Value.MeasureString(name.Value).X / 2f, Main.screenHeight * 0.45f), Main.DiscoColor with { A = 0 } * nameAlpha* alphaText);
		
		Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * glowAlpha);
	}
}