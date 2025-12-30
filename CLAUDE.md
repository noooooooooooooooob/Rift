# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 프로젝트 개요

**Rift**는 Unity 6와 Universal Render Pipeline(URP)을 사용하여 제작된 전술 턴제 RPG입니다. 10x5 타일 기반 그리드 전투 RPG. 전체적인 게임 진행은 시작 -> 파티 선택 -> 스테이지 진행 순으로 가며 스테이지는 전투, 이벤트, 상점, 보스 스테이지로 이루어져 있다. 전투는 파티 배치 -> 전투 -> 적 전멸 시 승리 순으로 진행된다.

**타겟 플랫폼:** PC


## 주요 명령어

**프로젝트 열기:**
```bash
# Unity Hub를 통해 열거나 Unity Editor를 직접 실행
unity -projectPath "C:\Users\User\Desktop\Unity\Rift"
```

**플레이 모드:** Unity Editor의 Play 버튼 또는 `Ctrl+P`

**빌드:** File → Build Settings → Build (또는 `Ctrl+B`)

### 테스트
- Unity 플레이 모드를 통한 수동 테스트가 주요 테스트 방법입니다
- 단위 테스트를 위한 Test Framework 패키지가 설치되어 있습니다 (`com.unity.test-framework`)

### Git
표준 git 워크플로우 사용. 최근 커밋들은 캐릭터 프리팹 및 렌더 설정에 대한 빈번한 반복 작업을 보여줍니다.

## 아키텍처 개요

### 핵심 시스템

**전투 시스템** (`Assets/Scripts/Battle/`)
- **Battle_Manager**: 10x5 그리드(`BattleGrid`)를 관리하고 플레이어/적 유닛 리스트를 추적하는 싱글톤
- **Turn_Manager**: 3단계(배치 → 전투 → 결과)로 구성된 상태 머신. 현재는 배치 단계만 구현됨
- **Deployment_Controller**: 드래그 앤 드롭을 통한 유닛 배치 처리. 유닛은 그리드의 왼쪽 절반(x < 5)에만 배치 가능
- **Tile**: 그리드 셀 컴포넌트. 위치와 점유 유닛을 추적

**유닛 시스템** (`Assets/Scripts/Unit/`)
- **Unit**: `CurrentTile` 참조와 `UnitData` ScriptableObject를 가진 핵심 엔티티
- **UnitStat**: 전투 스탯 구조체 (HP, ATK, DEF, SPD, AGI, LS, RST, EV, CA, DR, AP)
- **UnitData**: 유닛 설정을 위한 ScriptableObject (이름, 클래스, 스탯, 스프라이트, 초상화)

**카메라 시스템** (`Assets/Scripts/Camera/`)
- **CameraManager**: 두 가지 Cinemachine 카메라 상태 관리 (보드 뷰 = 우선순위 10, 포커스 뷰 = 우선순위 20)
- **MainCameraController**: 우클릭 드래그로 궤도 카메라 조작, 마우스 휠로 줌 조절(FOV 30-70), 스플라인 돌리 이동

**입력 시스템** (`Assets/Scripts/Always/`)
- **ClickController**: Unity의 새 Input System을 사용하는 전역 레이캐스트 핸들러
- **IClickable**: 클릭 가능 객체를 위한 인터페이스 (`void OnClick()`)
- **UnitClickable**: 유닛을 클릭 가능하게 만들고 카메라 포커스를 트리거

### 게임 흐름

```
씬 로드
  → CameraManager 초기화
  → Battle_Manager가 TileMapCreator로부터 10x5 그리드 생성
  → 타임라인 컷신 재생
  → CutSceneCallback.OnCutSceneEnd() 발동
  → Turn_Manager가 배치 단계로 전환
  → 플레이어가 유닛 배치 (왼쪽 절반만)
  → 모든 유닛 배치 완료 → 전투 단계 시작 (미구현)
```

### 주요 패턴

**싱글톤:**
- `Battle_Manager.instance`
- `CameraManager.instance`

**코루틴 상태 머신:**
```csharp
// Turn_Manager는 yield return을 사용한 무한 루프 사용
private IEnumerator State()
{
    while (true)
    {
        switch (currentPhase)
        {
            case BattlePhase.Deployment:
                yield return StartCoroutine(HandleDeploymentPhase());
                break;
            // ...
        }
    }
}
```

**인터페이스 기반 상호작용:**
```csharp
public interface IClickable { void OnClick(); }
```

**ScriptableObject 설정:**
- 새 유닛 데이터 생성: `Assets → Create → Scriptable Objects → UnitData`

## 파일 구조

```
Assets/
├── Scripts/
│   ├── Always/              # 전역 시스템 (ClickController, IClickable)
│   ├── Battle/              # Battle_Manager, Turn_Manager, Deployment_Controller, Tile
│   ├── Camera/              # CameraManager, MainCameraController
│   ├── Unit/                # Unit, UnitStat, SpriteBillboard
│   ├── Effects/             # ParticleEffectScaler, RadialFadeController
│   ├── Scene/               # Scene_Start_CutScene
│   ├── Else/                # ParallaxScrolling
│   ├── Method.cs            # 정적 유틸리티 함수 (전투 공식은 주석 처리됨)
│   ├── Game_Manager.cs      # 빈 플레이스홀더
│   └── Tile Map Creator.cs  # 그리드 생성 도구
├── Editor/                  # Sprite Baker, Tile Creator (커스텀 인스펙터)
├── Prefabs/
│   ├── Characters/          # Serian.prefab (Unit, UnitClickable, CinemachineCamera 포함)
│   └── Tile.prefab          # Tile 컴포넌트, SpriteRenderer, BoxCollider를 가진 그리드 타일
├── Scriptable Object/       # UnitData ScriptableObject 클래스 정의
├── Resources/
│   ├── Sprites/Character/   # 캐릭터 스프라이트 시트 (PSB 포맷)
│   └── 1 Backgrounds/       # 패럴랙스 배경 (7세트, 주간/야간)
├── Settings/                # PC_RPAsset.asset, Mobile_RPAsset.asset (URP 설정)
└── Scenes/
    └── SampleScene.unity    # 메인 씬
```

## 중요한 구현 세부사항

### 그리드 시스템
- `BattleGrid` 클래스에 정의된 고정 10x5 그리드
- 플레이어 유닛은 왼쪽 절반(x < 5)에 배치
- `TileMapCreator`가 타일을 생성하고 `Battle_Manager`가 참조

### 전투 시스템 (플레이스홀더)
- `Method.cs`의 모든 전투 공식이 주석 처리되어 있지만 의도된 설계를 보여줍니다:
  - `EV` 스탯(0-1 확률)을 사용한 명중/회피 시스템
  - 방어력 계산: `effectiveDef = DEF - AP` (방어구 관통)
  - 피해 감소: `DEF / (DEF + 100)`와 `DR` 스탯 결합
  - 생명력 흡수: `LS` 스탯 (최종 피해에 대한 0-1 배율)
  - 반격: `CA` 스탯 (0-1 확률)

### 레이어 관리
- 컷신 선택적 렌더링을 위해 "Focus" 레이어 사용
- 재귀적 레이어 할당을 위해 `Method.SetLayerRecursively(GameObject, layer)` 사용

### 입력 시스템
- **새로운 Unity Input System** 사용 (레거시 Input Manager 아님)
- 마우스 입력: `Mouse.current.leftButton.wasPressedThisFrame`
- 카메라 컨트롤: 우클릭 드래그, 스크롤 휠 줌

### 커스텀 에디터 도구
- **TileCreator**: `TileMapCreator`를 위한 커스텀 인스펙터 ("맵 생성" 및 "맵 초기화" 버튼 포함)
- **SpriteBaker**: 캐릭터 렌더를 PNG로 내보내는 컨텍스트 메뉴 도구

## 렌더 파이프라인

- PC와 모바일을 위한 별도 설정을 가진 **URP 17.3.0**
- `Assets/Resources/Shaders/`의 커스텀 셰이더
- 포스트 프로세싱 효과 (비네트를 위한 RadialFadeController)

## 주요 의존성

```json
{
  "com.unity.cinemachine": "3.1.5",
  "com.unity.timeline": "1.8.9",
  "com.unity.inputsystem": "1.17.0",
  "com.unity.render-pipelines.universal": "17.3.0",
  "com.unity.2d.animation": "13.0.2",
  "com.unity.2d.psdimporter": "12.0.1",
  "com.unity.animation.rigging": "1.4.0",
  "com.unity.ai.navigation": "2.0.9"
}
```

## 개발 중인 기능

여러 시스템이 미완성 플레이스홀더 상태입니다:
- **전투 단계** (Turn_Manager.HandleBattlePhase) - 주석 처리됨
- **결과 단계** (Turn_Manager.HandleResultPhase) - 주석 처리됨
- **전투 계산** (Method.cs) - 모든 피해 공식이 주석 처리됨
- **TurnGuage_Manager** - 빈 클래스
- **Unit_Drag** - 빈 클래스
- **Game_Manager** - 빈 클래스

이러한 시스템을 구현할 때는 의도된 전투 메커니즘을 위해 `Method.cs`의 주석 처리된 코드를 참조하세요.

## 개발 시 주의사항

- **싱글톤 접근**: 핵심 시스템에 접근하려면 `Battle_Manager.instance`와 `CameraManager.instance` 사용
- **그리드 좌표**: 타일 위치에는 항상 `Vector2Int` 사용
- **유닛 배치**: 배치 단계에서 그리드의 왼쪽 절반(`x < 5`)으로 제한
- **카메라 전환**: Cinemachine 우선순위를 직접 조작하지 말고 `CameraManager.SetBoardView()` 또는 `CameraManager.FocusOnCharacter(Transform)` 사용
- **클릭 가능 객체**: `IClickable` 인터페이스를 구현하고 레이캐스트 감지를 위한 적절한 Collider 추가
- **씬 초기화**: 전투 상태 머신은 `CutSceneCallback`이 호출하는 `Turn_Manager.OnCutSceneEnd()`를 통해 시작됨
