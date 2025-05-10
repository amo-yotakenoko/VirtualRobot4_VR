import keyboard
import sys
import time
from _device import *


print("test")
while True:
    # print(values)
    time.sleep(0.1)
    if keyboard.is_pressed('q'):
        sys.exit()
    a=0
    b=0
    if keyboard.is_pressed('up'):
        a+=400
        b+=400
    if keyboard.is_pressed('down'):
        a-=400
        b-=400
    if keyboard.is_pressed('left'):
        a+=400
        b-=400
    if keyboard.is_pressed('right'):
        a-=400
        b+=400
    # setvalue("A", "power",a)
    # setvalue("B", "power",b)
    A.power=a
    B.power=b
        

    if keyboard.is_pressed('z'):
        C.power=500
    elif keyboard.is_pressed('x'):
        C.power=-500
    else:
        C.power=0

