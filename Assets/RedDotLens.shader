Shader"Custom/RedDotLens"
{
    Properties
{
    [IntRange] _StencilID ("Stencil ID", Range(0,255)) = 0
}
    SubShader
    {
        Zwrite off
        ColorMask 0
        Cull off
    
        Stencil
        {
            Ref [_StencilID] // Stencil reference value
            Comp Always // Stencil comparison function (always, equal, etc.)
            Pass Replace // What happens to pixels that pass the stencil test
            Fail Keep
        }
    
        Pass 
        {

        }
    }
}
