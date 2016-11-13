org 100h

push value
call printInt
call exit

value dw -346

exit proc
    mov ax, 0x4c00
    int 21h
    ret
exit endp

print proc
    mov ah, 9
    int 21h
    ret
print endp

printInt proc ; (int num)
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
    
    cmp [bp+4], 0
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


