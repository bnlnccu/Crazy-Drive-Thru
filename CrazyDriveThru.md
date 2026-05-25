# 瘋狂得來速：認知任務切換實驗 (Crazy Drive-Thru)

## 壹、專案概述

本專案為 Unity 教學用 2D 遊戲模板（150 分鐘課堂）。表面上是速食店出餐遊戲，底層為心理學的「任務切換 (Task Switching)」實驗——與 Mind-Shuffle 使用相同的認知心理學機制，但以完全不同的遊戲形式呈現，強化「同一套 Toolkit 可用於不同遊戲」的復用概念。

**核心視覺：** 純 2D World Space，非 UGUI。左側生成餐點，向右側「外送紙袋」移動。上方有「經理指示燈」。

**與 Mind-Shuffle 的差異：**

| | Mind-Shuffle | Crazy Drive-Thru |
|---|---|---|
| UI 系統 | UGUI Canvas | 2D World Space |
| 玩家操作 | 按鈕點擊 | 點擊 Sprite / 拖曳 Sprite |
| 規則切換 | 依顏色 ↔ 依種類 | 薯條模式 ↔ 雞塊模式 |
| 技術重點 | UI Button + UnityEvent | Collider2D + OnMouseDown + OnTrigger |

---

## 貳、認知心理學機制 (Task Switching)

指示燈每 N 秒（可調）隨機切換規則：

- **狀態 A（黃燈 / 薯條模式）：** 只有「薯條」可以放行進紙袋。「雞塊」為干擾物，必須被玩家點擊排除。
- **狀態 B（紅燈 / 雞塊模式）：** 只有「雞塊」可以放行進紙袋。「薯條」為干擾物，必須被玩家點擊排除。

玩家大腦必須在兩種互斥規則中快速切換，產生**切換代價 (Switch Cost)**。

---

## 參、核心技術教學目標（基礎版必修）

所有互動元件均為 2D Sprite，**嚴禁使用 UGUI 的 Canvas 或 UI 事件**。

| 技術 | 用途 | 預計時間 |
|------|------|---------|
| Collider2D + OnMouseDown() | 玩家點擊排除錯誤餐點 | 30 分鐘 |
| Collider2D (IsTrigger) + OnTriggerEnter2D() | 紙袋偵測餐點抵達並判定對錯 | 15 分鐘 |
| Animator + SetTrigger() | 拋飛動畫、客訴動畫等視覺回饋 | 30 分鐘 |
| MoveToTarget2D（黑箱展示） | 餐點移動 / 輸送帶 | 5 分鐘 |

---

## 肆、進階教學開關 (Feature Toggle / DLC)

在 Inspector 上實作 bool 開關。**關閉時基礎版完美運行，開啟時解鎖進階玩法。**

### 基礎版核心：輸送帶（MoveToTarget2D）

輸送帶是基礎版必修機制，**非 DLC 開關**。餐點生成後自動沿輸送帶向右移動，玩家扮演「守門員」：
- 看到干擾物 → 點擊攔截丟棄
- 看到正確餐點 → 放行，讓它自己滑進紙袋得分

**專案初始狀態：** `MoveToTarget2D` 腳本的 `speed` 預設為 `2.0`，且移動邏輯已預先寫入——但外包工程師「不小心」漏掉了 `* Time.deltaTime`，導致餐點移動速度會隨電腦效能波動。此 Bug 將在 DLC B 階段由學生親手修復。

### DLC A：奧客的手動要求（解鎖拖曳）

```
Inspector: [SerializeField] bool enableDragMode = false;
```

- **關閉（預設）：** 用 OnMouseDown 點擊排除干擾物，正確餐點自動滑入紙袋
- **開啟：** 用 OnMouseDrag 拖曳餐點，OnMouseUp 放開後由物理引擎判定

⚠️ **技術限制：嚴禁使用 IDragHandler 介面！** 物件是 2D Sprite，使用 OnMouseDrag()。

**DLC A 判定機制：**
- FoodItem 加入 `public bool isDragging` 欄位
- `OnMouseDrag()`：`isDragging = true`，餐點追蹤滑鼠位置
- `OnMouseUp()`：`isDragging = false` → 啟動 0.1 秒延遲判定（`Invoke("CheckIfDelivered", 0.1f)`）
- **紙袋判定：** DeliveryBag 的 `OnTriggerStay2D` 偵測到 `isDragging == false` 時立即執行出餐判定（在 0.1 秒內搶先銷毀）
- **界外 Fallback：** 若 0.1 秒後物件仍存在（未被紙袋接收），視為 False Alarm → `SubtractScore` → `Destroy` → `NextTrial()`

預計教學時間：15 分鐘（有時間才教）

### DLC B：抓出效能殺手（Time.deltaTime 修正）

遊戲中 `MoveToTarget2D.cs` 的移動邏輯刻意少乘了 `Time.deltaTime`，導致餐點移動速度隨電腦效能波動（效能好的機器上會超光速飛過）。

學生任務：找出 Bug 並在 `transform.Translate(...)` 尾端補上 `* Time.deltaTime`。

**修正前：** 餐點超光速亂飛，速度不一致。
**修正後：** 餐點絲滑穩定移動，不受幀率影響。

預計教學時間：10 分鐘（有時間才教）

---

## 伍、GameToolkit 復用規範

**嚴格禁止在新腳本中重複造輪子。** 以下功能必須呼叫 `Assets/GameToolkit/` 現有 API：

| 功能 | 使用的 Toolkit | API |
|------|-------------|-----|
| 加分 | ScoreManager | `AddScore(int)` |
| 扣分 | ScoreManager | `SubtractScore(int)` |
| 回合推進 | TrialCounter | `NextTrial()` / `HasRemaining` |
| 反應時間 | ReactionTimeRecorder | `StartTiming()` / `StopTimingMs()` |
| 開場倒數 | CountdownTimer | `StartCountdown(Action)` |
| 分數重置 | ScoreManager | `ResetScore()` |

### 通訊協定 (Event Flow)

| 事件 | 觸發位置 | Toolkit 呼叫順序 |
|------|---------|-----------------|
| 餐點生成 | OrderManager | `reactionTimeRecorder.StartTiming()` |
| 正確出餐 / Hit（進紙袋，`CompareTag` 符合當前燈號） | DeliveryBag.OnTriggerEnter2D | `reactionTimeRecorder.StopTimingMs()` → `scoreManager.AddScore(10)` → `trialCounter.NextTrial()` |
| 錯誤出餐（進紙袋，`CompareTag` 不符當前燈號） | DeliveryBag.OnTriggerEnter2D | `reactionTimeRecorder.StopTimingMs()` → `scoreManager.SubtractScore(5)` → `trialCounter.NextTrial()` |
| 正確丟棄 / Correct Rejection（點擊排除干擾物） | FoodItem.OnMouseDown | `reactionTimeRecorder.StopTimingMs()` → `scoreManager.AddScore(5)` → `trialCounter.NextTrial()` |
| 錯誤丟棄 / False Alarm（點擊丟棄正確餐點） | FoodItem.OnMouseDown | `reactionTimeRecorder.StopTimingMs()` → `scoreManager.SubtractScore(5)` → 播放誤丟特效 → `trialCounter.NextTrial()` |
| 漏接 / Miss（餐點滑過紙袋，撞到 DestroyZone） | DestroyZone.OnTriggerEnter2D | `reactionTimeRecorder.StopTimingMs()` → 若為正確餐點 `scoreManager.SubtractScore(5)` → `Destroy` → `trialCounter.NextTrial()` |
| 遊戲結束 | OrderManager | `trialCounter.HasRemaining == false` → 顯示結算畫面 |

---

## 陸、腳本架構

### 1. OrderManager.cs（已寫好，學生不改）

**職責：** 燈號切換 + 餐點生成 + 流程控制。**絕對不包含** score、trial、reactionTime 變數。

**Inspector 面板：**
- `[SerializeField] float switchInterval = 10f`（燈號切換間隔秒數）
- `[SerializeField] GameObject[] foodPrefabs`（餐點 Prefab 清單）
- `[SerializeField] Transform spawnPoint`（生成位置）
- `[SerializeField] SpriteRenderer signalLight`（經理指示燈）
- `[SerializeField] bool enableDragMode = false`（DLC A 開關）
- Toolkit 參考：`ScoreManager`、`TrialCounter`、`CountdownTimer`、`ReactionTimeRecorder`

**公開 API：**
- `public static OrderManager Instance`（Singleton，在 `Awake()` 中賦值。其他腳本透過 `OrderManager.Instance` 存取）
- `CurrentState` 屬性 — 當前狀態（StateA / StateB）
- `DidSwitchThisTrial` 屬性 — 本回合是否剛切換

### 2. FoodItem.cs（★ 學生填空重點）

**掛載對象：** 薯條、雞塊等 2D Sprite Prefab。

**必要元件：** Collider2D（用於點擊偵測和觸發器碰撞）

**Inspector 面板：**
- Animator 參考
- （餐點類型透過 Unity 內建 `gameObject.tag` 識別："Fries" / "Nugget"，不使用自訂欄位）

**學生填空：**
- `OnMouseDown()` — 基礎版：用 `OrderManager.Instance.CurrentState` + `CompareTag()` 判斷是否為干擾物，是則播放拋飛動畫 + Destroy + 加分；否則為 False Alarm，扣分 + Destroy
- `OnMouseDrag()` — DLC A：拖曳邏輯（`isDragging = true`，追蹤滑鼠位置）
- `OnMouseUp()` — DLC A：`isDragging = false` + 0.1s 延遲判定（若未被紙袋接收則視為 False Alarm 自毀）

### 3. DeliveryBag.cs（★ 學生填空重點）

**掛載對象：** 畫面右側的外送紙袋。

**必要元件：** Collider2D（IsTrigger = true）

**學生填空：**
- `OnTriggerEnter2D()` — 用 `other.CompareTag()` 對比 `OrderManager.Instance.CurrentState` 判斷餐點是否符合當前燈號，正確 → `AddScore(10)`，錯誤 → `SubtractScore(5)`，最後 `Destroy` + `NextTrial()`
- `OnTriggerStay2D()` — DLC A 專用：偵測餐點 `isDragging == false`（玩家放手）時執行出餐判定

### 4. MoveToTarget2D.cs（基礎版預建，DLC B 抓 Bug）

**掛載對象：** 所有餐點 Prefab（基礎版即啟用，餐點生成後自動往右移動）。

**Inspector 面板：**
- `[SerializeField] float speed = 2f`
- `[SerializeField] Transform target`（移動目標，指向 DeliveryBag 或畫面右側）

**預建程式碼（刻意植入 Bug）：**
- `Update()` 內已寫好 `transform.Translate(Vector3.right * speed)` ——但**故意漏掉 `* Time.deltaTime`**

**學生填空（DLC B）：**
- 找出缺少 `* Time.deltaTime` 的 Bug 並修正

### 5. DestroyZone.cs（已寫好，學生不改）

**掛載對象：** 畫面最右側、DeliveryBag 後方的透明觸發區域。

**必要元件：** Collider2D（IsTrigger = true）

**職責：** 攔截所有滑過紙袋的餐點，防止物件洩漏。

**預建邏輯：**
- `OnTriggerEnter2D()` — 餐點撞到時：若為正確餐點（漏接 / Miss）→ `SubtractScore(5)` → `NextTrial()`；若為干擾物（玩家沒攔截但無傷大雅）→ 不扣分 → `NextTrial()`。最後 `Destroy` 餐點。

**註：** 所有餐點必須在離開畫面或進入紙袋後被銷毀，確保場上同一時間只有極少量物件，避免 ReactionTimeRecorder 對多物件計時產生干擾。

---

## 柒、填空清單與教學順序

**⚠️ 教學重點：按照下表順序進行，每完成一步就按 Play 體驗差異。**

### 前置說明

- **ScoreManager** 直接提供完整版（學生已在 Mind-Shuffle 填過 `AddScore` / `SubtractScore`，不重複教學）
- **MoveToTarget2D** 基礎版預建（輸送帶是核心機制，但刻意植入 Bug 作為 DLC B 教學素材）
- **DestroyZone** 預建（防禦性邏輯，學生不碰）
- 所有「填空前」= 空的 method body 或 TODO 註解佔位，Unity 編譯通過但功能無效

### 填空前的遊戲狀態（學生打開專案看到什麼）

- 場景正常載入，燈號會切換（紅/黃交替）
- 餐點在畫面左側生成，**沿輸送帶向右移動**（速度不穩定，因為缺少 `Time.deltaTime`）
- 點擊餐點 → 無反應（`OnMouseDown` body 是空的）
- 餐點滑進紙袋 → 穿過去，沒反應（`OnTriggerEnter2D` body 是空的）
- 餐點滑過紙袋 → 撞到 DestroyZone 被銷毀（預建，但不計分不推進回合）
- 分數永遠 0，Console 無輸出
- **遊戲可以「看」（餐點在動），但完全不能「玩」（沒有互動也沒有計分）**

### 投影片製作指引（給 Gemini）

每個填空製作**兩張連續的投影片**：

**第 1 張（題目頁）：** 顯示填空編號、檔案位置、TODO 提示、填完按 Play 會看到什麼效果。

**第 2 張（答案頁）：** 顯示標準答案的程式碼。

---

| 順序 | 編號 | 檔案 | 類型 | 學生要做什麼 | 按 Play 看到的差異 | 新技術 |
|------|------|------|------|-------------|-------------------|--------|
| 1 | I-1 | Unity Editor | Inspector | 在 Fries/Nugget Prefab 設定 Tag（"Fries"/"Nugget"），確認 Collider2D 存在 | Inspector Tag 從 Untagged 變成正確名稱 | Unity Tag 管理 |
| 2 | F-1 | FoodItem.cs | Code | `OnMouseDown()` 內填 `Destroy(gameObject);` | 點擊餐點 → 直接消失（無差別毀滅，先讓學生爽） | **OnMouseDown** |
| 3 | F-2 | FoodItem.cs | Code | 改寫 F-1：用 `OrderManager.Instance.CurrentState` + `CompareTag()` 判斷，干擾物 → `AddScore(5)` + `Destroy`；正確餐點 → `SubtractScore(5)` + `Destroy`（False Alarm）；兩者都呼叫 `NextTrial()` | 點干擾物 +5 分；誤點正確餐點 -5 分（從打地鼠進化成燒腦守門員） | **Singleton + CompareTag** |
| 4 | F-3 | FoodItem.cs | Code | 在 Destroy 前加 `animator.SetTrigger("Toss")`，改用 `Destroy(gameObject, 0.5f)` | 拋飛動畫播完才消失（視覺回饋升級） | **Animator.SetTrigger** |
| 5 | D-1 | DeliveryBag.cs | Code | `OnTriggerEnter2D()`：用 `CompareTag` 對比當前燈號，正確 → `AddScore(10)`，錯誤 → `SubtractScore(5)`，`Destroy` + `NextTrial()` | 餐點自動滑進紙袋 → 正確得分 / 錯誤扣分 | **OnTriggerEnter2D** |
| 6 | CSV | OrderManager.cs | Code | 組裝 `@DATA,Trial,State,IsSwitch,FoodType,Action,RT,Correct` 並 `Debug.Log` | Console 出現 @DATA 數據行 | 復習 |
| 7 | **DLC A** | FoodItem.cs | Code | `OnMouseDrag()` 追蹤滑鼠 + `OnMouseUp()` isDragging + 0.1s fallback（有時間才教） | 開啟拖曳模式，可拖曳餐點進紙袋 | **OnMouseDrag + OnMouseUp** |
| 8 | **DLC B** | MoveToTarget2D.cs | Debug | 找出缺少 `* Time.deltaTime` 的 Bug 並修正（有時間才教） | 超光速亂飛 → 絲滑穩定移動 | **Time.deltaTime** |

### 每步累積狀態

| 完成到 | 遊戲狀態 | 還缺什麼 |
|--------|---------|---------|
| I-1 | Tag 設好（畫面無變化） | 所有互動 |
| F-1 | 點什麼都消失（暴力模式） | 判斷、分數、動畫、紙袋 |
| F-2 | 守門員上線：干擾物 +5，誤點 -5 | 動畫、紙袋判定 |
| F-3 | 拋飛動畫回饋 | 紙袋判定 |
| D-1 | 紙袋能接餐判定計分 | CSV 數據 |
| CSV | Console 有實驗數據 | **基礎版完成！** |
| DLC A | 拖曳模式解鎖 | 輸送帶速度不穩 |
| DLC B | 輸送帶修好，速度穩定 | **全功能完成** |

---

## 捌、場景結構

```
CrazyDriveThru (Scene)
├── Main Camera
├── Background (2D Sprite)
├── GameManager (空物件)
│   ├── OrderManager (Singleton)
│   ├── ScoreManager (Toolkit)
│   ├── TrialCounter (Toolkit)
│   ├── CountdownTimer (Toolkit)
│   └── ReactionTimeRecorder (Toolkit)
├── SignalLight (2D Sprite, 經理指示燈)
├── SpawnPoint (空物件, 餐點生成位置, 畫面左側)
├── DeliveryBag (2D Sprite + Collider2D IsTrigger, 畫面右側)
├── DestroyZone (透明 Collider2D IsTrigger, DeliveryBag 後方, 漏接兜底)
├── UI Canvas (僅用於 HUD 文字顯示)
│   ├── ScoreText
│   ├── TrialText
│   ├── TimerText
│   └── ResultPanel
└── Prefabs（不在場景中）
    ├── Fries.prefab (Sprite + Collider2D + FoodItem + MoveToTarget2D + Animator, Tag="Fries")
    └── Nugget.prefab (Sprite + Collider2D + FoodItem + MoveToTarget2D + Animator, Tag="Nugget")
```

---

## 玖、專案資料夾結構

```
Assets/
├── GameToolkit/              # 從 Mind-Shuffle 復用，不修改
│   ├── Scripts/
│   │   ├── ScoreManager.cs
│   │   ├── CountdownTimer.cs
│   │   ├── TrialCounter.cs
│   │   └── ReactionTimeRecorder.cs
│   └── Examples/
├── Scripts/
│   └── DriveThru/
│       ├── OrderManager.cs       # 燈號 + 生成（框架，學生不改）
│       ├── FoodItem.cs           # ★ 學生填空（點擊 + 拖曳）
│       ├── DeliveryBag.cs        # ★ 學生填空（觸發器判定）
│       ├── DestroyZone.cs        # 漏接兜底（已寫好，學生不改）
│       └── MoveToTarget2D.cs     # 基礎版預建 / DLC B 抓 Bug
├── Prefabs/
│   ├── Fries.prefab
│   └── Nugget.prefab
├── Art/
│   └── DriveThru/                # 2D Sprite 素材
├── Audio/
│   ├── Correct.wav
│   └── Wrong.wav
└── Scenes/
    └── DriveThru.unity
```

---

## 拾、CSV 數據輸出

與 Mind-Shuffle 相同的 @DATA 前綴機制：

**表頭（Start 自動生成）：**
```
@DATA,Trial,State,IsSwitch,FoodType,Action,RT(ms),Correct
```

**欄位說明：**

| 欄位 | 說明 |
|------|------|
| Trial | 第幾回合 |
| State | 當前狀態（StateA / StateB） |
| IsSwitch | 本回合是否剛切換規則 |
| FoodType | 餐點類型（Fries / Nugget） |
| Action | 玩家動作（Delivered / Discarded / FalseAlarm / Miss） |
| RT(ms) | 反應時間 |
| Correct | 是否正確 |

---

## 拾壹、注意事項

### 2D World Space vs UGUI

本遊戲的互動物件（餐點、紙袋）全部使用 2D Sprite + Collider2D，**不是** UGUI 的 Button。`OnMouseDown` / `OnMouseDrag` 需要物件上有 Collider2D 才能觸發，且 Camera 需要照到物件即可。不需要額外掛載任何 EventSystem 相關元件。

### ScoreManager 的復用

ScoreManager 是 GameToolkit 的共用元件。學生已在 Mind-Shuffle 填過 `AddScore` / `SubtractScore`，本專案直接提供完整版，不重複填空。

### DLC 開關的防呆

DLC A（拖曳模式）開啟時，應停用 `OnMouseDown` 的點擊丟棄邏輯，避免兩種操作模式同時生效。FoodItem.cs 中需要檢查 `enableDragMode` 旗標。

### 燈號中途切換（這不是 Bug，是 Feature）

燈號可能在餐點已經生成、正在輸送帶上移動時切換。例如：薯條正在滑動，燈號突然從「薯條模式」切到「雞塊模式」，薯條瞬間從正確餐點變成干擾物。**這是刻意設計**，用於測量「抑制控制 (Inhibitory Control)」——玩家大腦必須中途壓制已啟動的放行反應，改為攔截。此情境會產生極高的認知負荷與珍貴的實驗數據。禁止將此行為視為 Bug 進行「修正」。

### 基礎版的計時模型

基礎版為離散 RT（Reaction Time）任務：每回合生成一個餐點，`ReactionTimeRecorder.StartTiming()` 在生成時啟動，玩家操作（點擊/放行/漏接）時停止。一次只有一個餐點在場上，計時器精確對應。若未來擴展為多餐點同時在場的模式，ReactionTimeRecorder 的單一計時器將不再精確，應改看整體正確率。
