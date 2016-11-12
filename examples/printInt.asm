org 100h

exit proc inline
    mov ax, 0x4c00
    int 21h
    ret
exit endp

print proc inline
    mov ah, 9
    int 21h
    ret
print endp

printInt proc inline ; (int num)
    push bp
    mov bp, sp
    mov ax, [bp+4]
    mov di, 5
    
    cmp ax, 0
    jg printInt_loop
    imul ax, -1
    
    printInt_loop:
        mov dx, 0
        mov bx, 10
        idiv bx
        add dx, 0x30
        dec di
        mov printIntData[di], dl
        cmp ax, 0   ; test ax, ax
    jnz printInt_loop
    
    add [bp+4], 0   ; cmp [bp+4], 0
    jg printInt_print
    dec di
    mov printIntData[di], 0x2d  ; -
    
    printInt_print:
    lea dx, printIntData
    add dx, di
    call print
    
    pop bp
    ret 2
    printIntData db '      $' ; reserved for number max 32767
printInt endp

push value
call printInt
call exit

value dw -346


