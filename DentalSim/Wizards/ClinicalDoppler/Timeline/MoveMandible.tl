<Save>
  <Saveable type="Medical.Muscles.MovementSequenceState, Simulation" id="4">
    <Float name="leftCPPosition">0.5182</Float>
    <Float name="rightCPPosition">0.5182</Float>
    <Vector3 name="movingTargetPosition">0, 0, 0</Vector3>
    <Float name="muscleForce">6</Float>
    <Float name="startTime">0.002720348</Float>
  </Saveable>
  <Saveable type="Medical.Muscles.MovementSequenceState, Simulation" id="5">
    <Float name="leftCPPosition">0.5401</Float>
    <Float name="rightCPPosition">0.5401</Float>
    <Vector3 name="movingTargetPosition">0, 0, 0</Vector3>
    <Float name="muscleForce">6</Float>
    <Float name="startTime">0.9314472</Float>
  </Saveable>
  <Saveable type="Medical.Muscles.MovementSequenceState, Simulation" id="6">
    <Float name="leftCPPosition">0.5766</Float>
    <Float name="rightCPPosition">0.5766</Float>
    <Vector3 name="movingTargetPosition">0, 0, 0</Vector3>
    <Float name="muscleForce">6</Float>
    <Float name="startTime">2.006529</Float>
  </Saveable>
  <Saveable type="Medical.Muscles.MovementSequenceState, Simulation" id="7">
    <Float name="leftCPPosition">0.6277</Float>
    <Float name="rightCPPosition">0.6277</Float>
    <Vector3 name="movingTargetPosition">0, 0, 0</Vector3>
    <Float name="muscleForce">6</Float>
    <Float name="startTime">2.959739</Float>
  </Saveable>
  <Saveable type="Medical.Muscles.MovementSequenceState, Simulation" id="8">
    <Float name="leftCPPosition">0.6934</Float>
    <Float name="rightCPPosition">0.6934</Float>
    <Vector3 name="movingTargetPosition">0, 0, 0</Vector3>
    <Float name="muscleForce">6</Float>
    <Float name="startTime">5.348205</Float>
  </Saveable>
  <Saveable type="Medical.Muscles.MovementSequenceState, Simulation" id="9">
    <Float name="leftCPPosition">0.6204</Float>
    <Float name="rightCPPosition">0.6204</Float>
    <Vector3 name="movingTargetPosition">0, 0, 0</Vector3>
    <Float name="muscleForce">6</Float>
    <Float name="startTime">6.583242</Float>
  </Saveable>
  <Saveable type="Medical.Muscles.MovementSequenceState, Simulation" id="10">
    <Float name="leftCPPosition">0.5474</Float>
    <Float name="rightCPPosition">0.5474</Float>
    <Vector3 name="movingTargetPosition">0, 0, 0</Vector3>
    <Float name="muscleForce">6</Float>
    <Float name="startTime">7.491839</Float>
  </Saveable>
  <Saveable type="Medical.Muscles.MovementSequenceState, Simulation" id="11">
    <Float name="leftCPPosition">0.5182</Float>
    <Float name="rightCPPosition">0.5182</Float>
    <Vector3 name="movingTargetPosition">0, 0, 0</Vector3>
    <Float name="muscleForce">6</Float>
    <Float name="startTime">8.677911</Float>
  </Saveable>
  <Saveable type="Medical.Muscles.MovementSequence, Simulation" id="3">
    <String name="Name">Protrusion Maximal</String>
    <Float name="Duration">10</Float>
    <Saveable name="State0" id="4" />
    <Saveable name="State1" id="5" />
    <Saveable name="State2" id="6" />
    <Saveable name="State3" id="7" />
    <Saveable name="State4" id="8" />
    <Saveable name="State5" id="9" />
    <Saveable name="State6" id="10" />
    <Saveable name="State7" id="11" />
  </Saveable>
  <Saveable type="Medical.PlaySequenceAction, Standalone" id="2">
    <Saveable name="MovementSequence" id="3" />
    <Bool name="PauseOnStop">False</Bool>
    <Float name="StartTime">0</Float>
    <Float name="Duration">259200</Float>
  </Saveable>
  <Saveable type="Medical.ActionSequencer`1[[Medical.TimelineAction, Standalone, Version=2.0.0.3052, Culture=neutral, PublicKeyToken=null]], Standalone" id="1">
    <Saveable name="Actions0" id="2" />
  </Saveable>
  <Saveable type="Medical.Timeline, Standalone" id="0">
    <Bool name="Fullscreen">True</Bool>
    <Saveable name="Sequencer" id="1" />
  </Saveable>
</Save>