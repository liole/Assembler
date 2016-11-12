org 100h

exit proc inline
    mov ax, 0x4c00
    int 21h
    ret
exit endp

abs proc inline ; (ax := |ax|)
    sub ax, 0
    jg abs_return
    imul ax, -1
    abs_return:
    ret
abs endp

setVideoMode proc inline
    mov     ah, 0       ; set display mode function.
    mov     al, 13h     ; mode 13h = 320x200 pixels, 256 colors.
    int     10h         ; set it!
    ret
setVideoMode endp

drawPixel proc inline ; (int x, int y, color)
    push bp
    mov bp, sp
    
    mov     cx, [bp+8]      ; column
    mov     dx, [bp+6]      ; row
    mov     al, [bp+4]      ; white
    mov     ah, 0ch     ; put pixel
    int     10h         ; draw pixel
    
    pop bp
    ret 6
drawPixel endp

drawLine proc inline ; (int x1, int y1, int x2, int y2, int color)
    push bp
    mov bp, sp
    
    ; [bp+12] = x1
    ; [bp+10] = y1
    ; [bp+8] = x2
    ; [bp+6] = y2
    ; [bp+4] = color
    
    mov ax, [bp+8]  ; x2
    cmp ax, [bp+12] ; x1
    jl drawLine_xLZ
        push 1
        jmp drawLine_xDiffEnd
    drawLine_xLZ:
        push -1
    drawLine_xDiffEnd:
        
    mov ax, [bp+6]  ; y2
    cmp ax, [bp+10] ; y1
    jl drawLine_yLZ
        push 1
        jmp drawLine_yDiffEnd
    drawLine_yLZ:
        push -1
    drawLine_yDiffEnd:
    
    mov si, [bp+12] ; x1
    mov di, [bp+10] ; y1
    
    drawNextPixel:
        push si
        push di
        push [bp+4] ; color
        call drawPixel
        
        delta proc inline
            mov ax, [bp+8]  ; x2
            sub ax, [bp+12] ; x1
            mov bx, di
            sub bx, [bp+10] ; y1
            imul bx
            mov cx, ax
            
            mov ax, [bp+6]  ; y2
            sub ax, [bp+10] ; y1
            mov bx, si
            sub bx, [bp+12] ; x1
            imul bx
            sub ax, cx
            call abs
            ret
        delta endp
        
        add si, [bp-2]
        call delta
        sub si, [bp-2]
        
        push ax
        
        add di, [bp-4]
        call delta
        sub di, [bp-4]
        
        pop bx
        cmp ax, bx
        jl incY
        
        incX:
            add si, [bp-2]
        jmp nextIteration
        
        incY:
            add di, [bp-4]
        nextIteration:
        
        mov ax, si
        imul ax, [bp-2]
        mov bx, [bp+8]  ; x2
        imul bx, [bp-2]
        cmp ax, bx
    jl drawNextPixel
        
        mov ax, di
        imul ax, [bp-4]
        mov bx, [bp+6]  ; y2
        imul bx, [bp-4]
        cmp ax, bx
    jl drawNextPixel
    
    add sp, 4   ; x/y diff
    
    pop bp
    ret 10
drawLine endp


call setVideoMode

push x1
push y1
push x2
push y2
push red
call drawLine

push x2
push y2
push x3
push y3
push green
call drawLine

push x3
push y3
push x1
push y1
push blue
call drawLine

call exit

x1 dw 160
y1 dw 50

x2 dw 100
y2 dw 170

x3 dw 220
y3 dw 120

red   dw 4
green dw 2
blue  dw 1




