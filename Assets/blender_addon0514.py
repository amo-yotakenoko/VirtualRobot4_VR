# 20250514 Updated!


import bpy
import json
import os

def gltf_export_with_modifier():
    print("=== GLTFエクスポート処理開始 ===")

    original_objects = []
    duplicated_objects = []

    print("オブジェクトを複製してモディファイアを適用中...")
    templetes_collection = bpy.data.collections.get("templetes")

    # 最初に対象オブジェクトをMESH, EMPTY, CAMERA, LIGHT, ARMATUREに拡張
    target_objects = [
        obj for obj in bpy.context.scene.objects
        if obj.type in {'MESH', 'EMPTY', 'CAMERA', 'LIGHT', 'ARMATURE'}
        and (templetes_collection is None or obj.name not in templetes_collection.objects)
    ]
    # 全オブジェクトの選択をクリア
    bpy.ops.object.select_all(action='DESELECT')

    for obj in target_objects:
        print(f"処理中: {obj.name}")
        original_objects.append(obj)

        # 複製
        dup_obj = obj.copy()
        dup_obj.data = obj.data.copy() if obj.type in {'MESH', 'ARMATURE'} else None
        bpy.context.collection.objects.link(dup_obj)

        # モディファイア適用（MESHにのみ適用）
        if obj.type == 'MESH':
            bpy.context.view_layer.objects.active = dup_obj
            dup_obj.select_set(True)
            for mod in dup_obj.modifiers:
                try:
                    bpy.ops.object.modifier_apply(modifier=mod.name)
                    print(f"  モディファイア適用: {mod.name}")
                except Exception as e:
                    print(f"  モディファイア適用失敗: {mod.name} on {dup_obj.name}: {e}")

        duplicated_objects.append(dup_obj)

        # 複製したオブジェクトだけを選択対象にする
        dup_obj.select_set(True)

        # 元のオブジェクトは選択解除し、非表示にする
        obj.select_set(False)
        obj.hide_render = True
        obj.hide_viewport = True

    # 複製されたオブジェクトだけ選択状態にする
    for dup in duplicated_objects:
        dup.select_set(True)

    # ファイル名とパス
    file_name = bpy.path.display_name_from_filepath(bpy.data.filepath) + ".gltf"
    export_path = os.path.join(os.path.dirname(bpy.data.filepath), file_name)

    print(f"GLTFファイルを書き出します: {export_path}")
    bpy.ops.export_scene.gltf(
        filepath=export_path,
        export_cameras=True,
        export_lights=True,
        use_selection=True  # 選択されたオブジェクト（複製されたオブジェクトのみ）をエクスポート
    )

    print("複製オブジェクトを削除中...")
    for dup in duplicated_objects:
        bpy.data.objects.remove(dup, do_unlink=True)

    print("元のオブジェクトを表示状態に戻します。")
    for obj in original_objects:
        obj.hide_render = False
        obj.hide_viewport = False

    print("=== GLTFエクスポート完了 ===")
    
    

class ExportGLTF(bpy.types.Operator):
    bl_idname = "object.export_gltf"
    bl_label = "Export as glTF"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        print("Exporting as glTF")
        
        
        gltf_export_with_modifier()
        
        with open('_device.py', 'w') as f:
            f.write("""
# 自動生成されるのでここにプログラムを書かないで!!
import sys
class Motor:
    def __init__(self,name):
        self.name=name
        self._power=0
    @property
    def power(self):
        return self._power
    @power.setter
    def power(self, value):
        if self._power != value:
            print(f"set: {self.name}.power = {value}")
            sys.stdout.flush() 
            self._power = value

class Servo:
    def __init__(self, name):
        self.name = name
        self._angle = 0
    @property
    def angle(self):
        return self._angle
    @angle.setter
    def angle(self, value):
        if self._angle != value:
            print(f"set: {self.name}.angle = {value}")
            sys.stdout.flush() 
            self._angle = value

class Light:
    def __init__(self, name):
        self.name = name
        self._intensity = 0
    @property
    def intensity(self):
        return self._intensity
    @intensity.setter
    def intensity(self, value):
        if self._intensity != value:
            print(f"set: {self.name}.intensity = {value}")
            sys.stdout.flush() 
            self._intensity = value
class Camera:
    def __init__(self, name):
        self.name = name
class Orbitcamera:
    def __init__(self, name):
        self.name = name
""")
            written_instances=[]
            for obj in bpy.context.scene.objects:
                device=extract_json(obj.name)
                if device:
                    instance=  f"{device['name']} = {device['type'].capitalize()}(\"{device['name']}\")\n"
                    if instance in written_instances:
                        continue
                    written_instances.append(instance)
                    f.write(instance)
        
        return {'FINISHED'}

class AddMotor(bpy.types.Operator):
    bl_idname = "object.add_motor"
    bl_label = "Add Motor"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        print("Adding Motor")
        original_object = bpy.data.objects.get("motor")
        new_object = original_object.copy()
        new_object.data = original_object.data.copy()
        new_object.name = '{"name":"A","type":"motor"}'
        bpy.context.collection.objects.link(new_object)
        bpy.context.view_layer.objects.active = new_object
        
        new_object.select_set(True)
        
        return {'FINISHED'}
    
    
class AddServo(bpy.types.Operator):
    bl_idname = "object.add_servo"
    bl_label = "Add Servo"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        print("Adding Servo")
        original_object = bpy.data.objects.get("motor")
        new_object = original_object.copy()
        new_object.data = original_object.data.copy()
        new_object.name = '{"name":"A","type":"servo"}'
        bpy.context.collection.objects.link(new_object)
        bpy.context.view_layer.objects.active = new_object
        
        new_object.select_set(True)
        return {'FINISHED'}
    
    
class AddTpsCamera(bpy.types.Operator):
    bl_idname = "object.add_tps_camera"
    bl_label = "Add TPS Camera"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
    
        original_object = bpy.data.objects.get("orbitCamera")
        new_object = original_object.copy()
        new_object.name = '{"name": "camera", "type": "orbitCamera"}'
        bpy.context.collection.objects.link(new_object)
        bpy.context.view_layer.objects.active = new_object
        
        new_object.select_set(True)
        return {'FINISHED'}



def object_selection_changed(scene, depsgraph):
    context = bpy.context
    selected_objects = context.selected_objects
    if selected_objects:
        selected_object = selected_objects[0]
        json_data = extract_json(selected_object.name)
        if json_data:
            props = context.scene.my_tool
            for key, value in json_data.items():
                if hasattr(props, key):
                    setattr(props, key, value)
    context.area.tag_redraw()

class MyProperties(bpy.types.PropertyGroup):
    name: bpy.props.StringProperty(name="name", update=lambda self, context: update_object_name(context))
    type: bpy.props.StringProperty(name="type", update=lambda self, context: update_object_name(context))

def update_object_name(context):
    selected_objects = bpy.context.selected_objects
    if selected_objects:
        selected_object = selected_objects[0]
        props = context.scene.my_tool
        json_data = {key: getattr(props, key) for key in ['name', 'type']}
        new_name = json.dumps(json_data)
        selected_object.name = new_name

class VirtualRobotPanel(bpy.types.Panel):
    bl_label = "VirtualRobot4forBlender"
    bl_idname = "PT_VirtualRobotPanel"
    bl_space_type = 'VIEW_3D'
    bl_region_type = 'UI'
    bl_category = 'VirtualRobot4forBlender'

    def draw(self, context):
        layout = self.layout
        layout.operator("object.export_gltf", text="Export as glTF")
        layout.label(text="Add")
        layout.operator("object.add_motor", text="モーター")
        layout.operator("object.add_servo", text="サーボモーター")
        layout.operator("object.add_tps_camera", text="TPS視点カメラ")
        layout.label(text="selected")
        
        for selected_object in bpy.context.selected_objects:
            json_data = extract_json(selected_object.name)
            if json_data:
                props = context.scene.my_tool
                for key in json_data:
                    if hasattr(props, key):
                        layout.prop(props, key, text=key)
                layout.operator("object.update_object_name", text="Update Name")

class UpdateObjectNameOperator(bpy.types.Operator):
    bl_idname = "object.update_object_name"
    bl_label = "Update Object Name"
    bl_options = {'REGISTER', 'UNDO'}

    def execute(self, context):
        selected_objects = bpy.context.selected_objects
        if selected_objects:
            selected_object = selected_objects[0]
            props = context.scene.my_tool
            json_data = {key: getattr(props, key) for key in ['name', 'type']}
            new_name = json.dumps(json_data)
            selected_object.name = new_name
        return {'FINISHED'}

def extract_json(input_string):
    indent = 0
    split_json = ""
    json_detected = False

    for c in input_string:
        if c == '{':
            indent += 1
            json_detected = True
        if indent > 0:
            split_json += c
        if c == '}':
            indent -= 1
            if indent == 0 and json_detected:
                break

    if json_detected:
        try:
            extracted_json = json.loads(split_json)
            return extracted_json
        except json.JSONDecodeError:
            return None
    else:
        return None

def register():
    bpy.utils.register_class(ExportGLTF)
    bpy.utils.register_class(AddMotor)
    bpy.utils.register_class(AddServo)
    bpy.utils.register_class(AddTpsCamera)
    bpy.utils.register_class(MyProperties)
    bpy.types.Scene.my_tool = bpy.props.PointerProperty(type=MyProperties)
    bpy.utils.register_class(VirtualRobotPanel)
    bpy.utils.register_class(UpdateObjectNameOperator)
    bpy.app.handlers.depsgraph_update_post.append(object_selection_changed)

def unregister():
    bpy.utils.unregister_class(ExportGLTF)
    bpy.utils.unregister_class(AddMotor)
    bpy.utils.unregister_class(AddServo)
    bpy.utils.unregister_class(AddTpsCamera)
    bpy.utils.unregister_class(MyProperties)
    bpy.utils.unregister_class(VirtualRobotPanel)
    bpy.utils.unregister_class(UpdateObjectNameOperator)
    bpy.app.handlers.depsgraph_update_post.remove(object_selection_changed)

if __name__ == "__main__":
    register()
