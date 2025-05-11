import keyboard
import sys
import time

values={}

def setvalue(identifier, command, value):
    if not f"{identifier}.{command}" in values or values[f"{identifier}.{command}"]!= value:
        print(f"set: {identifier}.{command} = {value}")
    sys.stdout.flush() 
    values[f"{identifier}.{command}"]=value


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
    setvalue("A", "power",a)
    setvalue("B", "power",b)
        

    
    if keyboard.is_pressed('z'):
        setvalue("C", "power", 500)
    elif keyboard.is_pressed('x'):
        setvalue("C", "power", -500)
    else:
        setvalue("C", "power", 0) 

    # if keyboard.is_pressed('c'):
    #     setvalue("D", "power", 100)
    # elif keyboard.is_pressed('v'):
    #     setvalue("D", "power", -100)
    # else:
    #     setvalue("D", "power", 0) 
        



    if keyboard.is_pressed('p'):
        if not f"L.intensity" in values or values["L.intensity"]>0:
            setvalue("L", "intensity", 0)
        else:
            setvalue("L", "intensity", 1)
        time.sleep(0.5)

