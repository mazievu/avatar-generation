import os

# --- Các hàm tạo nội dung cho tệp C# ---

def get_monobehaviour_content(class_name):
    """Tạo nội dung cho một lớp kế thừa từ MonoBehaviour."""
    return f"""using UnityEngine;

public class {class_name} : MonoBehaviour
{{
    // Start is called before the first frame update
    void Start()
    {{
        
    }}

    // Update is called once per frame
    void Update()
    {{
        
    }}
}}
"""

def get_plain_csharp_content(class_name):
    """Tạo nội dung cho một lớp C# thông thường."""
    return f"""using System;

public class {class_name}
{{
    
}}
"""


# --- Cấu trúc dự án ---

project_structure = {
    "Assets": {
        "Art": {
            "UI": {
                "Common": [],
                "Icons": [],
                "PathOfLife": [],
                "Business": []
            },
            "Avatars": {
                "Manifests": [],
                "Layers": []
            },
            "Fonts": [],
            "Audio": []
        },
        "Scripts": {
            "Core": {
                "Domain": {
                    "Characters": ["Character.cs", "CharacterStats.cs"],
                    "Events": ["GameEvent.cs", "EventChoice.cs", "EventSystem.cs"],
                    "Education": ["SchoolOption.cs", "UniversityMajor.cs", "EducationService.cs"],
                    "Career": ["CareerTrack.cs", "CareerService.cs"],
                    "Economy": ["EconomyService.cs"],
                    "Business": ["Business.cs", "BusinessService.cs"],
                    "Game": ["GameState.cs", "GameEngine.cs", "PausePolicy.cs"]
                },
                "Services": ["ISaveStore.cs", "JsonFileStore.cs", "ILocalization.cs", "LocalizationManager.cs", "IRandom.cs", "IClock.cs"],
                "Data": ["Constants.cs", "UnlockableFeature.cs", "PathNodes.cs"]
            },
            "Presentation": {
                "Boot": ["Bootstrap.cs", "GameLoop.cs"],
                "UI": {
                    "GameUIController.cs": None,
                    "Modal": ["ModalManager.cs", "ComicPanel.cs", "EventPanel.cs", "SchoolChoicePanel.cs", "ClubChoicePanel.cs", "UniversityChoicePanel.cs", "UniversityMajorPanel.cs", "CareerChoicePanel.cs", "UnderqualifiedPanel.cs", "LoanPanel.cs", "PromotionPanel.cs", "InstructionsPanel.cs", "SettingsPanel.cs", "UnlockNotificationPanel.cs"],
                    "Panels": ["FamilyTreePanel.cs", "BusinessPanel.cs", "AssetsPanel.cs", "PathOfLifePanel.cs", "GameLogPanel.cs"],
                    "Widgets": ["ChoiceButton.cs", "StatBar.cs", "IncomeFloatText.cs", "StarEffect.cs", "SmokeEffect.cs", "IconLibrary.cs"],
                    "Avatar": ["AvatarPreviewController.cs", "AvatarBuilderPanel.cs"]
                },
                "Integration": ["AudioManager.cs", "TimeScaler.cs", "NotificationBridge.cs"]
            },
            "Tests": {
                "Core": ["GameEngineTests.cs", "EventResolverTests.cs"]
            }
        },
        "Resources": {
            "Localization": ["en.json", "vi.json"],
            "Databases": {
                "Events": ["newborn.json", "elementary.json", "middleschool.json", "highschool.json", "university.json", "workinglife.json", "retired.json"],
                "Clubs": ["clubs.json"],
                "Careers": ["career_ladder.json"],
                "Education": ["school_options.json"],
                "PathOfLife": ["path_nodes.json"],
                "Features": ["unlockable_features.json"]
            },
            "UI": {
                "IconAtlas.spriteatlas": None,
                "Prefabs": {
                    "UI": {
                        "MainUI.prefab": None,
                        "BottomNav.prefab": None,
                        "ComicPanel.prefab": None,
                        "ChoiceButton.prefab": None,
                        "Panels": ["PathOfLifePanel.prefab", "BusinessPanel.prefab"]
                    }
                }
            }
        },
        "Plugins": []
    }
}

def create_project_structure(base_path, structure):
    """
    Tạo đệ quy các thư mục và tệp dựa trên cấu trúc đã cho.
    Ghi nội dung khởi tạo cho các tệp C#.
    """
    for name, content in structure.items():
        current_path = os.path.join(base_path, name)
        
        # Xử lý các mục là tệp (ví dụ: "GameUIController.cs": None)
        if content is None or '.' in name:
            filename = name
            file_path = current_path
            
            # Bỏ qua nếu giá trị là None và không phải tệp (trường hợp đặc biệt)
            if '.' not in filename:
                continue

            # Tạo tệp
            file_content = "" # Nội dung mặc định là trống
            if filename.endswith(".cs"):
                class_name = filename[:-3] # Bỏ phần mở rộng ".cs"
                # Phân tách đường dẫn để kiểm tra thư mục cha
                path_parts = file_path.split(os.sep)
                
                # Kiểm tra nếu nằm trong thư mục Presentation
                is_monobehaviour = False
                for part in path_parts:
                    if part == "Presentation":
                        is_monobehaviour = True
                        break
                
                if is_monobehaviour:
                    file_content = get_monobehaviour_content(class_name)
                else:
                    file_content = get_plain_csharp_content(class_name)

            with open(file_path, 'w', encoding='utf-8') as f:
                f.write(file_content)
            print(f"  Đã tạo tệp: {file_path}")
            continue

        # Xử lý các mục là thư mục
        os.makedirs(current_path, exist_ok=True)
        print(f"Đã tạo thư mục: {current_path}")
        
        if isinstance(content, dict):
            create_project_structure(current_path, content)
        elif isinstance(content, list):
            for filename in content:
                # Gọi lại hàm cho từng tệp trong danh sách
                create_project_structure(current_path, {filename: None})


if __name__ == "__main__":
    base_directory = os.getcwd()
    print(f"Bắt đầu tạo cấu trúc dự án trong: {base_directory}")
    create_project_structure(base_directory, project_structure)
    print("\nHoàn tất việc tạo cấu trúc thư mục và tệp!")
    print("\nLưu ý: Các tệp không phải C# (như .prefab, .json) được tạo dưới dạng tệp trống.")