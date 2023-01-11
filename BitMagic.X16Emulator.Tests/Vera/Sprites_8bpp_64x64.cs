using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera.Display;

[TestClass]
public class Sprites_8bpp_64x64
{
    [TestMethod]
    public async Task Normal()
    {
        var emulator = new Emulator();

        emulator.LoadSprite(@"Vera\Images\testsprite_8bpp_64x64.png", ImageHelper.ColourDepthSprite.Depth_8bpp, 64, 64, 0);

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R41
            .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
            .org $810
                sei

                lda #02
                sta DC_BORDER

                lda #$40        ; 1bpp bitmap
                sta L0_CONFIG
                sta L1_CONFIG
    
                lda #$41    ; Sprites
                sta DC_VIDEO     

                ; set colour 0
                lda #$01
                sta ADDRx_H
                lda #$fa
                sta ADDRx_M
                stz ADDRx_L

                lda #$12
                sta DATA0

                ; setup sprite
                lda #$11
                sta ADDRx_H
                lda #$fc
                sta ADDRx_M
                stz ADDRx_L
            
                stz DATA0   ; address
                lda #$80    ; 8bpp 
                sta DATA0
    
                lda #1     ; x
                sta DATA0
                stz DATA0

                lda #1     ; y
                sta DATA0
                stz DATA0
            
                lda #$04    ; depth
                sta DATA0

                lda #$f0    ; 64x64
                sta DATA0

                ldx #128
                stx DC_VSCALE  
                stx DC_HSCALE
  

                lda #01
                sta IEN
                wai
                sta ISR     ; clear interrupt and wait for second frame
                wai

                stp
        ", emulator);
        
        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_64x64.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_64x64.png");
    }

    [TestMethod]
    public async Task Normal_Hflip()
    {
        var emulator = new Emulator();

        emulator.LoadSprite(@"Vera\Images\testsprite_8bpp_64x64.png", ImageHelper.ColourDepthSprite.Depth_8bpp, 64, 64, 0);

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R41
            .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
            .org $810
                sei

                lda #02
                sta DC_BORDER

                lda #$40        ; 1bpp bitmap
                sta L0_CONFIG
                sta L1_CONFIG
    
                lda #$41    ; Sprites
                sta DC_VIDEO     

                ; set colour 0
                lda #$01
                sta ADDRx_H
                lda #$fa
                sta ADDRx_M
                stz ADDRx_L

                lda #$12
                sta DATA0

                ; setup sprite
                lda #$11
                sta ADDRx_H
                lda #$fc
                sta ADDRx_M
                stz ADDRx_L
            
                stz DATA0   ; address
                lda #$80    ; 8bpp 
                sta DATA0
    
                lda #1     ; x
                sta DATA0
                stz DATA0

                lda #1     ; y
                sta DATA0
                stz DATA0
            
                lda #$04+1   ; depth + hflip
                sta DATA0

                lda #$f0    ; 64x64
                sta DATA0

                ldx #128
                stx DC_VSCALE  
                stx DC_HSCALE
  

                lda #01
                sta IEN
                wai
                sta ISR     ; clear interrupt and wait for second frame
                wai

                stp
        ", emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_64x64_hflip.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_64x64_hflip.png");
    }

    [TestMethod]
    public async Task Normal_Vflip()
    {
        var emulator = new Emulator();

        emulator.LoadSprite(@"Vera\Images\testsprite_8bpp_64x64.png", ImageHelper.ColourDepthSprite.Depth_8bpp, 64, 64, 0);

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R41
            .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
            .org $810
                sei

                lda #02
                sta DC_BORDER

                lda #$40        ; 1bpp bitmap
                sta L0_CONFIG
                sta L1_CONFIG
    
                lda #$41    ; Sprites
                sta DC_VIDEO     

                ; set colour 0
                lda #$01
                sta ADDRx_H
                lda #$fa
                sta ADDRx_M
                stz ADDRx_L

                lda #$12
                sta DATA0

                ; setup sprite
                lda #$11
                sta ADDRx_H
                lda #$fc
                sta ADDRx_M
                stz ADDRx_L
            
                stz DATA0   ; address
                lda #$80    ; 8bpp 
                sta DATA0
    
                lda #1     ; x
                sta DATA0
                stz DATA0

                lda #1     ; y
                sta DATA0
                stz DATA0
            
                lda #$04+2   ; depth + vflip
                sta DATA0

                lda #$f0    ; 64x64
                sta DATA0

                ldx #128
                stx DC_VSCALE  
                stx DC_HSCALE
  

                lda #01
                sta IEN
                wai
                sta ISR     ; clear interrupt and wait for second frame
                wai

                stp
        ", emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_64x64_vflip.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_64x64_vflip.png");
    }


    [TestMethod]
    public async Task Normal_HVflip()
    {
        var emulator = new Emulator();

        emulator.LoadSprite(@"Vera\Images\testsprite_8bpp_64x64.png", ImageHelper.ColourDepthSprite.Depth_8bpp, 64, 64, 0);

        await X16TestHelper.Emulate(@"
            .machine CommanderX16R41
            .byte $0C, $08, $0A, $00, $9E, $20, $32, $30, $36, $34, $00, $00, $00, $00, $00
            .org $810
                sei

                lda #02
                sta DC_BORDER

                lda #$40        ; 1bpp bitmap
                sta L0_CONFIG
                sta L1_CONFIG
    
                lda #$41    ; Sprites
                sta DC_VIDEO     

                ; set colour 0
                lda #$01
                sta ADDRx_H
                lda #$fa
                sta ADDRx_M
                stz ADDRx_L

                lda #$12
                sta DATA0

                ; setup sprite
                lda #$11
                sta ADDRx_H
                lda #$fc
                sta ADDRx_M
                stz ADDRx_L
            
                stz DATA0   ; address
                lda #$80    ; 8bpp 
                sta DATA0
    
                lda #1     ; x
                sta DATA0
                stz DATA0

                lda #1     ; y
                sta DATA0
                stz DATA0
            
                lda #$04+3   ; depth + hvflip
                sta DATA0

                lda #$f0    ; 64x64
                sta DATA0

                ldx #128
                stx DC_VSCALE  
                stx DC_HSCALE
  

                lda #01
                sta IEN
                wai
                sta ISR     ; clear interrupt and wait for second frame
                wai

                stp
        ", emulator);

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_64x64_hvflip.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_64x64_hvflip.png");
    }
}