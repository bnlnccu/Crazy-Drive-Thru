# Crazy Drive-Thru: Teaching Slides Content (v0 ~ v8)

> For Gemini: Each step below should be TWO slides.
> **Slide 1 (Task):** Show the step number, purpose, file location, what to do, and what pressing Play will show AFTER completing it.
> **Slide 2 (Answer):** Show the correct code or Inspector settings, plus a screenshot description of the expected result.

---

## Step S-1: Add Background to Scene

### Purpose
Learn how to add a 2D Sprite to the scene and set its rendering order so it stays behind all other objects.

### Steps
1. In the **Project** window, navigate to `Assets/Art/DriveThru/`
2. Drag `background.png` into the **Hierarchy** window (or directly into the Scene view)
3. A new GameObject named "background" will appear in the Hierarchy
4. Select it, look at the **Inspector** on the right side
5. Find the **Sprite Renderer** component
6. Change **Order in Layer** from `0` to `-10`
7. (Optional) Adjust the Transform Position to `(0, 0, 1)` to center it

### Answer
**Inspector settings:**
- Sprite Renderer > Order in Layer: **-10**
- Transform Position: **(0, 0, 1)**

**Press Play result:** The drive-thru kitchen background fills the entire screen. The signal light and UI text (Score, Trial) are visible on top of it. No food appears yet because we haven't connected the Prefabs.

---

## Step S-2a: Place the Delivery Bag

### Purpose
Learn how to place a game object in the scene and control its visual layering so it appears IN FRONT of food items (food will slide behind the bag when delivered).

### Steps
1. In the **Project** window, navigate to `Assets/Art/DriveThru/`
2. Drag `DeliveryBag.png` into the **Hierarchy** window
3. Select the new "DeliveryBag" GameObject
4. In the **Inspector**, set the following:
   - **Sprite Renderer > Order in Layer:** `5` (higher = drawn on top)
   - **Transform Position:** `(4, -0.8, 0)` (right side of the screen, slightly below center)
   - **Transform Scale:** `(1.5, 2.2, 1)` (adjust size to match the scene)

### Answer
**Inspector settings:**
- Sprite Renderer > Order in Layer: **5**
- Transform Position: **(4, -0.8, 0)**
- Transform Scale: **(1.5, 2.2, 1)**

**Press Play result:** The paper bag appears on the right side of the kitchen counter. It's drawn in front of everything else (Order in Layer = 5 > food's default 0), so food will visually slide behind it later. Still no food spawning yet.

---

## Step S-2b: Make the Bag Detect Food (Collider + Script)

### Purpose
Learn how to add a 2D physics collider as a **Trigger** (detects objects passing through without blocking them) and attach a game script to a GameObject.

### Steps
1. Select the "DeliveryBag" GameObject in the Hierarchy
2. In the Inspector, click **Add Component**
3. Search for `Box Collider 2D` and add it
4. Check the **Is Trigger** checkbox (this makes it detect objects passing through instead of blocking them)
5. Click **Add Component** again
6. Search for `DeliveryBag` (our script) and add it

### Answer
**Inspector components (in order):**
- Transform
- Sprite Renderer (Order in Layer = 5)
- **Box Collider 2D** (Is Trigger = **checked**, Size = 1 x 1)
- **Delivery Bag (Script)**

**Press Play result:** Visually the same as before. But now the bag has physics detection. When food slides into the bag area later, `OnTriggerEnter2D` in the DeliveryBag script will fire. (You won't see a difference yet because food isn't spawning.)

---

## Step I-1: Set Tags on Food Prefabs

### Purpose
Learn Unity's **Tag system** -- a built-in way to label GameObjects so scripts can identify them using `CompareTag()`. We need to tag Fries and Nugget Prefabs so the game can tell them apart.

### Steps
1. In the **Project** window, navigate to `Assets/Prefabs/`
2. Click on `Fries.prefab` to select it
3. In the Inspector at the top, find the **Tag** dropdown (currently shows "Untagged")
4. Change it to **Fries**
5. Click on `Nugget.prefab`
6. Change its Tag to **Nugget**

### Answer
- Fries.prefab > Tag: **Fries**
- Nugget.prefab > Tag: **Nugget**

**Press Play result:** No visible change yet. But the Prefabs now have correct Tags that `CompareTag("Fries")` and `CompareTag("Nugget")` will use to determine which food is which.

---

## Step S-3: Connect Prefabs to the Order Manager

### Purpose
Learn how to assign Prefab references in the Inspector. The OrderManager needs to know WHICH Prefabs to spawn -- we tell it by dragging our Fries and Nugget Prefabs into its `foodPrefabs` array.

### Steps
1. In the **Hierarchy**, expand `GameManager` and select `OrderManager`
2. In the Inspector, find **Food Spawn > Food Prefabs**
3. It currently shows `Size: 0` (empty array)
4. Change Size to `2`
5. Drag `Fries.prefab` from Project window into Element 0
6. Drag `Nugget.prefab` from Project window into Element 1

### Answer
**Inspector settings:**
- Food Prefabs > Size: **2**
- Element 0: **Fries**
- Element 1: **Nugget**

**Press Play result:** After the 3-2-1 countdown, food items start spawning on the left side and **move to the right at extremely high speed** (they fly across the screen almost instantly). This is because the movement code has a deliberate bug (missing `Time.fixedDeltaTime`) that we'll fix later in DLC B. Clicking food does nothing yet. Score stays at 0.

---

## Step F-1: Click to Destroy Food

### Purpose
Learn `OnMouseDown()` -- Unity's built-in method that fires when the player clicks on a 2D object that has a Collider2D. Your first real interaction with the game world!

### File
`Assets/Scripts/DriveThru/FoodItem.cs` -- find the `OnMouseDown()` method

### Steps
1. Open `FoodItem.cs` in your code editor
2. Find the `OnMouseDown()` method (around line 13)
3. Below the two `if` guard lines, find `// ===== TODO F-1 =====`
4. Replace the TODO comment with: `Destroy(gameObject);`
5. Save the file, switch back to Unity, and press Play

### Answer
```csharp
private void OnMouseDown()
{
    if (isBeingDestroyed) return;
    if (OrderManager.Instance != null && OrderManager.Instance.EnableDragMode) return;

    Destroy(gameObject);  // <-- NEW
}
```

**Press Play result:** Click any food item and it **instantly disappears**! Every food you click vanishes -- whether it's the correct one or a distractor. No scoring, no animation, no judgment. You're a food-destroying machine. We'll add intelligence in the next step.

---

## Step F-2: Add Judgment Logic

### Purpose
Learn `CompareTag()` and the **Singleton pattern** (`OrderManager.Instance`). Instead of blindly destroying everything, now the game checks: is this food a distractor (should be removed) or the correct food (shouldn't be touched)?

### File
`Assets/Scripts/DriveThru/FoodItem.cs` -- `OnMouseDown()` method

### Steps
1. **Delete** the `Destroy(gameObject);` line you added in F-1
2. Replace `// ===== TODO F-2 =====` and ALL its comment lines with the judgment logic below
3. Key concept: `OrderManager.Instance.CurrentState` tells you the current rule:
   - `StateA` (yellow light) = Fries should pass, Nugget is distractor
   - `StateB` (red light) = Nugget should pass, Fries is distractor
4. Use `CompareTag("Fries")` to check if this food is Fries

### Answer
```csharp
private void OnMouseDown()
{
    if (isBeingDestroyed) return;
    if (OrderManager.Instance != null && OrderManager.Instance.EnableDragMode) return;

    // NEW: Judgment logic
    GameState state = OrderManager.Instance.CurrentState;
    bool isDistractor = false;

    if (state == GameState.StateA)
        isDistractor = !CompareTag("Fries");   // In StateA, anything that's NOT Fries is a distractor
    else
        isDistractor = !CompareTag("Nugget");  // In StateB, anything that's NOT Nugget is a distractor

    ScoreManager sm = FindObjectOfType<ScoreManager>();

    if (isDistractor)
        sm.AddScore(5);        // Correct Rejection: +5 points
    else
        sm.SubtractScore(5);   // False Alarm: -5 points (you clicked the correct food!)

    Destroy(gameObject);
}
```

**Press Play result:** Now clicking has consequences!
- Click a **distractor** (wrong food) -> **Score +5** (correct rejection)
- Click the **correct food** -> **Score -5** (false alarm -- you shouldn't have clicked it!)
- The signal light at the top shows yellow (StateA: pass Fries) or red (StateB: pass Nugget)
- The "PASS: Fries" / "PASS: Nugget" label tells you which food to let through

---

## Step F-3: Add Toss Animation

### Purpose
Learn `Animator.SetTrigger()` -- triggering animations from code. Instead of food vanishing instantly, it now plays a "toss" animation (spin + shrink) before being destroyed.

### File
`Assets/Scripts/DriveThru/FoodItem.cs` -- `OnMouseDown()` method

### Steps
1. **Delete** the `Destroy(gameObject);` line from F-2
2. Replace `// ===== TODO F-3 =====` and ALL its comment lines with the animation + trial completion code below
3. Key concepts:
   - `anim.enabled = true` -- the Animator starts disabled to avoid interfering with movement
   - `anim.SetTrigger("Toss")` -- triggers the "Toss" animation state
   - `Destroy(gameObject, 0.5f)` -- waits 0.5 seconds before destroying (so animation can play)
   - `OnTrialComplete()` -- tells OrderManager this trial is done, triggers next food spawn

### Answer
```csharp
private void OnMouseDown()
{
    if (isBeingDestroyed) return;
    if (OrderManager.Instance != null && OrderManager.Instance.EnableDragMode) return;

    GameState state = OrderManager.Instance.CurrentState;
    bool isDistractor = false;

    if (state == GameState.StateA)
        isDistractor = !CompareTag("Fries");
    else
        isDistractor = !CompareTag("Nugget");

    ScoreManager sm = FindObjectOfType<ScoreManager>();

    if (isDistractor)
        sm.AddScore(5);
    else
        sm.SubtractScore(5);

    // NEW: Stop movement, play animation, delayed destroy
    var move = GetComponent<MoveToTarget2D>();
    if (move != null) move.enabled = false;

    if (anim != null)
    {
        anim.enabled = true;
        anim.SetTrigger("Toss");
    }
    isBeingDestroyed = true;

    string action = isDistractor ? "Discarded" : "FalseAlarm";
    OrderManager.Instance.OnTrialComplete(gameObject.tag, action, isDistractor);
    OrderManager.Instance.NotifyFoodDestroyed(gameObject);
    Destroy(gameObject, 0.5f);
}
```

**Press Play result:** Click a food item and it **spins and shrinks** over 0.5 seconds before disappearing! The conveyor belt stops for that food, the score updates, and a new food spawns immediately. You can hear correct/wrong sound effects. The Trial counter advances (e.g., Trial 1/30 -> Trial 2/30).

---

## Step D-1: Delivery Bag Judgment

### Purpose
Learn `OnTriggerEnter2D()` -- Unity's method that fires when a Rigidbody2D object enters a Trigger collider. When food slides into the paper bag, we check if it's the correct food or not.

### File
`Assets/Scripts/DriveThru/DeliveryBag.cs` -- find the `OnTriggerEnter2D()` method

### Steps
1. Open `DeliveryBag.cs` in your code editor
2. Find `// ===== TODO D-1 =====` inside `OnTriggerEnter2D()`
3. Replace the TODO comment block with the judgment logic below
4. This is very similar to F-2, but:
   - Correct delivery = **+10 points** (bigger reward for letting the right food through)
   - Wrong delivery = **-5 points**
   - Uses `other.CompareTag()` because `other` is the food that entered the trigger

### Answer
```csharp
private void OnTriggerEnter2D(Collider2D other)
{
    FoodItem food = other.GetComponent<FoodItem>();
    if (food == null) return;
    if (food.isDragging) return;

    var move = other.GetComponent<MoveToTarget2D>();
    if (move != null) move.enabled = false;

    // NEW: Judgment logic
    GameState state = OrderManager.Instance.CurrentState;
    bool correct = false;

    if (state == GameState.StateA)
        correct = other.CompareTag("Fries");
    else
        correct = other.CompareTag("Nugget");

    ScoreManager sm = FindObjectOfType<ScoreManager>();
    if (correct)
        sm.AddScore(10);       // Correct delivery: +10
    else
        sm.SubtractScore(5);   // Wrong delivery: -5

    OrderManager.Instance.OnTrialComplete(other.gameObject.tag, "Delivered", correct);
    OrderManager.Instance.NotifyFoodDestroyed(other.gameObject);

    StartCoroutine(FallIntoBag(other.gameObject));
}
```

**Press Play result:** The game is now fully playable!
- Food slides right on the conveyor belt
- If you **don't click** the correct food, it slides into the bag -> **+10 points**
- If you **don't click** a distractor, it slides into the bag -> **-5 points** (you should have intercepted it!)
- If you **click** a distractor -> **+5 points** (correct rejection)
- If you **click** the correct food -> **-5 points** (false alarm)
- Food entering the bag plays a "fall in" animation (slides down and shrinks)
- The base game is functionally complete! You can play all 30 trials.

---

## Step CSV: Data Output to Console

### Purpose
Learn `Debug.Log()` for data collection. In psychology experiments, we need to record every trial's data. This step outputs structured CSV data to Unity's Console so researchers can copy it for analysis.

### File
`Assets/Scripts/DriveThru/OrderManager.cs` -- find `OnTrialComplete()` method

### Steps
1. Open `OrderManager.cs` in your code editor
2. Find `// ===== TODO CSV =====` inside `OnTrialComplete()` (around line 190)
3. Replace the TODO comment block with the CSV logging code below
4. The variables you need are already prepared above the TODO:
   - `trialCounter.CurrentTrial`, `CurrentState`, `DidSwitchThisTrial`
   - `foodTag`, `action`, `rt` (reaction time), `correct`

### Answer
```csharp
// Replace the TODO block with:
string csvLine = string.Format("{0},{1},{2},{3},{4},{5},{6}",
    trialCounter.CurrentTrial, CurrentState, DidSwitchThisTrial,
    foodTag, action, rt.ToString("F1"), correct);
collectedDataLines.Add(csvLine);
Debug.Log("@DATA," + csvLine);
```

**Press Play result:** Open the **Console** window (Window > General > Console). As you play, each trial outputs a line like:
```
@DATA,0,StateA,False,Fries,Delivered,1523.4,True
@DATA,1,StateA,False,Nugget,Discarded,892.1,True
@DATA,2,StateB,True,Fries,FalseAlarm,1102.5,False
```
After the game ends (30 trials), you can click **Export CSV** on the result screen to save the data as a `.csv` file.

**The base game is now 100% complete!** The remaining steps are DLC (advanced content, time permitting).

---

## Step DLC A: Drag Mode (Advanced, Time Permitting)

### Purpose
Learn `OnMouseDrag()` and `OnMouseUp()` -- instead of clicking to destroy food, you can now drag food items with the mouse and drop them into the bag manually.

### Prerequisite
In OrderManager's Inspector, check **Enable Drag Mode** to activate this DLC.

### File
`Assets/Scripts/DriveThru/FoodItem.cs` -- `OnMouseDrag()` and `OnMouseUp()` methods

### Steps
1. Find `// ===== TODO DLC A-1 =====` inside `OnMouseDrag()`
2. Add the drag logic: set `isDragging = true` and move the food to follow the mouse
3. Find `// ===== TODO DLC A-2 =====` inside `OnMouseUp()`
4. Add the release logic: set `isDragging = false` and start a 0.1s timer to check if the food landed in the bag

### Answer
```csharp
private void OnMouseDrag()
{
    if (!OrderManager.Instance.EnableDragMode) return;
    if (isBeingDestroyed) return;

    // NEW
    isDragging = true;
    Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
}

private void OnMouseUp()
{
    if (!OrderManager.Instance.EnableDragMode) return;
    if (isBeingDestroyed) return;

    // NEW
    isDragging = false;
    Invoke("CheckIfDelivered", 0.1f);
}
```

**Press Play result (with Enable Drag Mode checked):**
- Food items can be **picked up and dragged** with the mouse
- Drop food **into the bag** -> it triggers delivery judgment (same as D-1)
- Drop food **outside the bag** -> after 0.1 seconds, it self-destructs as a False Alarm (-5 points)
- The conveyor belt still moves food you're not holding

---

## Step DLC B: Fix the Speed Bug (Advanced, Time Permitting)

### Purpose
Learn about **frame-rate independent movement** using `Time.fixedDeltaTime`. The outsourced developer "forgot" to multiply by delta time, causing food to move at different speeds depending on computer performance.

### File
`Assets/Scripts/DriveThru/MoveToTarget2D.cs` -- `FixedUpdate()` method

### Steps
1. Open `MoveToTarget2D.cs`
2. Find `// ===== TODO DLC B =====` (around line 24)
3. Look at the line below it: `rb.MovePosition(rb.position + direction * speed);`
4. The bug: `speed` is multiplied directly without considering frame time. On a 60fps computer, food moves 4 units per physics frame = 200 units/second. On a 30fps computer, it's 120 units/second. Inconsistent!
5. Fix: multiply by `Time.fixedDeltaTime` to make movement frame-rate independent

### Answer
```csharp
// BEFORE (buggy):
rb.MovePosition(rb.position + direction * speed);

// AFTER (fixed):
rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
```

**Press Play result:**
- **Before fix:** Food flies across the screen at warp speed, barely visible
- **After fix:** Food glides smoothly at a consistent, playable speed (~4 units/second) regardless of computer performance

**Congratulations! The game is now fully complete!**
