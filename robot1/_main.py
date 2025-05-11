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
        a+=200
        b+=200
    if keyboard.is_pressed('down'):
        a-=200
        b-=200
    if keyboard.is_pressed('left'):
        a+=200
        b-=200
    if keyboard.is_pressed('right'):
        a-=200
        b+=200
    # setvalue("A", "power",a)
    # setvalue("B", "power",b)
    A.power=a
    B.power=b
        

    
    if keyboard.is_pressed('z'):
        C.angle=-250
    else:
        C.angle=0

    if keyboard.is_pressed('x'):
        D.angle=-80
    else:
        D.angle=0
  

    # if keyboard.is_pressed('c'):
    #     D.angle=100
    # elif keyboard.is_pressed('v'):
    #     D.angle=-100
    # else:
    #     D.angle=0
        



    if keyboard.is_pressed('p'):
        if L.intensity>0:
            L.intensity=0
        else:
            L.intensity=1
        time.sleep(0.5)

