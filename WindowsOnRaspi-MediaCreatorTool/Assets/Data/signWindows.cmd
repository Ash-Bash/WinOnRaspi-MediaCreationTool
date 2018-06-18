@Echo Off
bcdboot i:\windows /s p: /f UEFI
bcdedit /store P:\EFI\Microsoft\Boot\bcd /set {default} testsigning on
bcdedit /store P:\EFI\Microsoft\Boot\bcd /set {default} nointegritychecks on
pause
exit