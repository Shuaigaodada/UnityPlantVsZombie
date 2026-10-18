# 植物大战僵尸 Unity 复刻（UnityPlantVsZombie）

用 Unity 2022.3 复刻《植物大战僵尸》白天草坪关卡的 2D 塔防练习项目：收集阳光、点选卡片、在 5×9 草坪上种植植物，用豌豆射手和向日葵抵挡沿行推进的僵尸。

## 简介

- 单场景演示：`Assets/Scenes/game.unity`（Build Settings 中唯一启用的场景）。
- 核心循环：天空掉落阳光并自动收集 → 点击底部卡片选择植物 → 点击草坪格子种植 → 向日葵产阳光、豌豆射手朝本行僵尸射击 → 僵尸沿行前进、啃食植物，掉头后死亡。
- 草坪为 5 行 × 9 列网格，由 `LawnGrid` 读取场景中的行/列子节点与 `BoxCollider2D` 初始化；`GameManager` 负责阳光资源、植物列表、卡片选择与游戏状态；编辑器扩展 `LabelDrawer` 让 Inspector 中的中文字段标签更易读。

## 目录结构

```text
Unity/                           Unity 工程根目录（用 Unity Hub 打开此目录）
├── Assets/
│   ├── Animation/               向日葵、豌豆射手、僵尸、阳光等动画与控制器
│   ├── Editor/LabelDrawer.cs    Inspector 中文标签绘制扩展
│   ├── Image/                   背景、卡片、植物、僵尸、UI、特效与新增素材
│   ├── Prefab/                  豌豆子弹、豌豆射手、阳光、向日葵、僵尸等预制体
│   ├── Resources/               运行时加载资源
│   ├── Scenes/game.unity        唯一游戏场景
│   ├── Scripts/                 玩法脚本
│   │   ├── GameManager.cs       阳光、卡片选择、游戏状态
│   │   ├── LawnGrid.cs          5×9 草坪网格与种植占用
│   │   ├── ClickManager.cs      鼠标点击与卡片/格子交互
│   │   ├── Card.cs              植物卡片
│   │   ├── Plant.cs、Enemy.cs   植物与敌人基类
│   │   ├── Sunlight.cs          阳光掉落与收集
│   │   ├── PeaShooter/          豌豆射手与子弹
│   │   ├── Sunflower/           向日葵产阳光
│   │   └── Zombie/              僵尸移动、攻击与掉头
│   └── TextMesh Pro/            文本渲染资源
├── Packages/manifest.json       包依赖（2D 功能包、TMP、Timeline 等）
├── ProjectSettings/             工程设置（ProjectVersion.txt 记录编辑器版本）
└── Plant vs Zombie.sln          Visual Studio / Rider 解决方案
```

## 环境要求

- Unity Hub 与 Unity 编辑器 **2022.3.62f3c1**（依据 `ProjectSettings/ProjectVersion.txt`；该版本为 2022.3 LTS 定制修订，尽量用同版本打开以免重新导入）。
- 包依赖见 `Packages/manifest.json`：`com.unity.feature.2d` 2.0.1、`com.unity.textmeshpro` 3.0.7、`com.unity.timeline` 1.7.7、`com.unity.ugui` 1.0.0 等，由 Unity 自动还原。
- 无需额外 SDK；Windows 下用 Unity Hub 打开工程即可。

## 启动方式（Windows PowerShell）

1. 在 Unity Hub 安装 2022.3.62f3c1 编辑器。
2. 打开工程目录（Unity Hub → Add → 选择 `E:\Projects\Plant-vs-Zombie\Unity`），等待资源导入完成。
3. 打开场景 `Assets/Scenes/game.unity`，点击编辑器顶部的 Play 按钮运行。

也可以直接用命令行启动编辑器并打开工程：

```powershell
& 'C:\Program Files\Unity\Hub\Editor\2022.3.62f3c1\Editor\Unity.exe' -projectPath 'E:\Projects\Plant-vs-Zombie\Unity'
```

首次打开会重新导入 `Assets` 下的图片、动画与预制体，耗时取决于机器性能；导入完成后按上方步骤进入 Play 模式。

## 使用方法

- 开局有初始阳光，阳光会从天而降，点击阳光收集（自动收集逻辑见 `Sunlight.cs` 与 `GameManager`）。
- 点击底部卡片选中植物，再点击草坪格子种植；阳光不足或格子已占用时无法种植。
- 向日葵周期性产出阳光，豌豆射手自动向本行僵尸发射豌豆；僵尸走到植物前会停下啃食，生命归零掉头并淡出消失。
- 植物与僵尸的数值（阳光消耗、冷却、伤害、血量、移速）都在 Inspector 中通过序列化字段配置，调整后重新运行即可验证。

## 已知限制

- 这是学习性质的复刻项目，只有一张白天草坪场景，未包含夜晚、泳池、屋顶、迷你游戏、关卡选择、存档等完整游戏内容。
- 美术、动画与音效素材来自《植物大战僵尸》原作，版权归原权利方所有，仅供学习与本地运行，不应对外分发或商用。
- 工程未提供打包好的可执行文件；需要自行用 Unity 构建 Windows 版本。
- 数值与手感未做平衡性调校，仅用于验证种植、射击、阳光与僵尸推进的核心循环。
