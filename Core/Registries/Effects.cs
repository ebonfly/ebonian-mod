namespace EbonianMod.Core.Registries;

public class EffectSystem : ModSystem
{
    public override void Load() => Effects.LoadEffects();
}

public static class Effects
{
    public static Asset<Effect> bloom, colorQuant, softBloom, Tentacle, TentacleBlack, TentacleRT, ScreenDistort,
        SpriteRotation, TextGradient, TextGradient2, TextGradientY, BeamShader, Lens, Test1,
        Test2, LavaRT, Galaxy, CrystalShine, HorizBlur, TrailShader, RTAlpha, Crack, Blur,
        RTOutline, metaballGradient, metaballGradientNoiseTex, invisibleMask, PullingForce,
        displacementMap, waterEffect, spherize, flame, flameGround, starlightRiver;
    
    
    public static void LoadEffects()
    {
        static Asset<Effect> LoadEffect(string path) => Request<Effect>("EbonianMod/Assets/Effects/" + path); // line of code STOLEN *DIRECTLY* from ONE OF ZEN'S MODS. THANKS, IDIOT!
        bloom = LoadEffect("bloom");
        colorQuant = LoadEffect("colorQuant");
        Test1 = LoadEffect("Test1");
        HorizBlur = LoadEffect("horizBlur");
        Blur = LoadEffect("Blur");
        Crack = LoadEffect("crackTest");
        RTAlpha = LoadEffect("RTAlpha");
        RTOutline = LoadEffect("RTOutline");
        CrystalShine = LoadEffect("CrystalShine");
        TextGradient = LoadEffect("TextGradient");
        TextGradient2 = LoadEffect("TextGradient2");
        TextGradientY = LoadEffect("TextGradientY");
        Test2 = LoadEffect("Test2");
        Galaxy = LoadEffect("Galaxy");
        LavaRT = LoadEffect("LavaRT");
        SpriteRotation = LoadEffect("spriteRotation");
        BeamShader = LoadEffect("Beam");
        Lens = LoadEffect("Lens");
        Tentacle = LoadEffect("Tentacle");
        TentacleRT = LoadEffect("TentacleRT");
        ScreenDistort = LoadEffect("DistortMove");
        TentacleBlack = LoadEffect("TentacleBlack");
        TrailShader = LoadEffect("TrailShader");
        metaballGradient = LoadEffect("metaballGradient");
        metaballGradientNoiseTex = LoadEffect("metaballGradientNoiseTex");
        invisibleMask = LoadEffect("invisibleMask");
        PullingForce = LoadEffect("PullingForce");
        displacementMap = LoadEffect("displacementMap");
        waterEffect = LoadEffect("waterEffect");
        spherize = LoadEffect("spherize");
        flame = LoadEffect("flameEffect");
        flameGround = LoadEffect("flameGroundEffect");
        starlightRiver = LoadEffect("starlightRiver");
    }
}