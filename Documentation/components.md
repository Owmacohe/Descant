[toc]



## Node components

- **Event call**
  - *Method*
  - *Parameters*
- **Statistic change**
  - *Actor*
  - *Statistic*
- **Statistic reveal**
  - *Actor*
  - *Statistic*



### Choice node components

- **Timed choice**
  - *Time allowed for choice*
  - *Timer visible?*
- **Locked choice**
  - *Topic lock?*
  - *Statistic lock?*
  - *Timed lock?*
  - *Re-attempt lock?*
  - *Relationship lock?*
- **Changeable choice**
  - *Statistic change?*
  - *Timed change?*
  - *Re-attempt change?*
  - *Relationship change?*



### Response node components

- **Interruptable**
  - *Resume conversation after?*
- **Add known topic**
  - *Keyword*
- **Changeable response**
  - *Statistic change?*
  - *Timed change?*
  - *Re-attempt change?*
  - *Relationship change?*



## End node components

- **Ending type**
  - *Good*
  - *Bad*
  - *Custom*



## Actor components

- Player
  - **Known topics**
    - *Keywords*
- NPC
  - **Limited responses**
    - *Number of responses*
    - *Number of re-attempts*
    - *Statistic change for re-attempts?*
  - **Limited conversations**
    - *Number of conversations*
    - *Number of re-attempts*
    - *Statistic change for re-attempts?*
- Both
  - **Movement freedom**
    - *Does distance end conversation?*
  - **Statistics**
    - *Type*
    - *Initial value*
    - *Dependant methods*
  - **Actor relationship**
    - *Favourability*
    - *High/low unsurpassably thresholds*



## Conversation components

- **Conversation recap**
  - *Statistic changes*
  - *Relationship changes*
  - *Topics learned*
  - *Conversation length*
  - *Ending type*