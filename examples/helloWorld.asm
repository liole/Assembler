org 100h

; prit line
mov ah, 9
lea dx, msg
int 21h

; exit
mov ax, 0x4c00
int 21h

msg  db 'Hello world!'
     db 10
     db 13
     db '$'