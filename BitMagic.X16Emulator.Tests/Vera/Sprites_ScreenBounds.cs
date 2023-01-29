using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitMagic.X16Emulator.Tests.Vera.Display;

[TestClass]
public class Sprites_ScreenBounds
{
    [TestMethod]
    public async Task AboveScreen()
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

                lda #$f0     ; y
                sta DATA0
                lda #$ff
                sta DATA0
            
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_64x64_abovescreen.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_64x64_abovescreen.png");
    }

    [TestMethod]
    public async Task LeftScreen()
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
    
                lda #$f0    ; x
                sta DATA0
                lda #$ff
                sta DATA0

                lda #1      ; y
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_64x64_leftscreen.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_64x64_leftscreen.png");
    }

    [TestMethod]
    public async Task AllOffLeftScreen()
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
    
                lda #$00    ; x
                sta DATA0
                lda #$ff
                sta DATA0

                lda #1      ; y
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_64x64_alloffleftscreen.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_64x64_alloffleftscreen.png");
    }

    [TestMethod]
    public async Task RightScreen()
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
    
                lda #$6c    ; x
                sta DATA0
                lda #$02
                sta DATA0

                lda #1      ; y
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_64x64_rightscreen.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_64x64_rightscreen.png");
    }



    [TestMethod]
    public async Task AllOffRightScreen()
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
    
                lda #$ff    ; x
                sta DATA0
                lda #$02
                sta DATA0

                lda #1      ; y
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

        //emulator.SaveDisplay(@"C:\Documents\Source\BitMagic\BitMagic.X16Emulator.Tests\Vera\Images\sprites_8bpp_64x64_alloffrightscreen.png");

        emulator.CompareImage(@"Vera\Images\sprites_8bpp_64x64_alloffrightscreen.png");
    }
}