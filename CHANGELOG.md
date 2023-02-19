## [1.7.1] - 2022-02-19

### Added:
- Added support for Cinemachine 3.0.0.

### Fixed:
- Fixed WASD not being associated with keyboard when creating new input actions, thus not allowing the player to move when using PlayerInput.
- Fixed not being able to undo creation of controller when using Cinemachine.
- Fixed a normal camera being created when using Cinemachine.
- Fixed CinemachineBrain not being created when creating a new controller with Cinemachine enabled.
- Fixed logs being printed when zooming.
- Fixed step cycle not increasing when using a custom step cycle.
- Fixed obsolete warnings in 2023.1+.
- Fixed constant garbage allocation in GoldPlayerUI.
- Fixed IMGUI editor for AudioItem.
- Fixed several UI toolkit editor issues for AudioItem.
- Fixed a lot of garbage allocation in editors.
- Fixed spelling mistake in GoldPlayerGraphics.

## [1.7.0] - 2022-08-13

### Added:
- Added raycast ground check as an alternative to sphere ground check.
- Added gizmos in the scene view for ground checks.
- Added interaction limits on Gold Player Interactable, along with a On Reached Max Interactions event.
- Added new editor GUI for several components. They are now more uniform and adapts to what values are enabled.
- Added option to only rotate the camera without rotating the root transform.
- Added support for using Vector2 with legacy input manager.
- Added support for using legacy Gold Player Input component with Gold Player Input System if 'Both' is set in input backend.
- Added zoom feature.
- Added separate predictable values for crouching and standing up, along with curves to control the lerp.
- Added curves to field of view kick.
- Added support for player input in input system component.

### Changes:
- Revamped how Gold Player project settings are saved. Old settings will be upgraded.
- Changed Drain Stamina When and Kick When to flags instead for better control. Will most likely not upgrade very well if you've used these values.
- Mouse Sensitivity and Mouse Damping have been renamed to Look Sensitivity and Look Damping.
- All virtual methods and protected fields have been turned private.
- Bob target is now a child of camera head by default due to the change regarding only rotating the camera.
- When Cinemachine is enabled, the newly created player object will have a virtual camera instead of a normal camera.
- Stamina now has control over unscaled time by itself.
- **[BREAKING]** Target Camera has been moved from field of view kick to player camera instead.
- `ShouldJump` in `PlayerMovement` is now get only and returns if the player should jump with stamina, air jumps, etc, considered. PressedJump has been added as a replacement for the old behavior.
- Field of view kick times are now predictable. **If you're upgrading from an older version, you might need to change your old values!**
- Input now uses hashes for lookups instead. **BREAKING** Enable/DisableInput(int) on GoldPlayerInputSystem is now called Enable/DisableInputIndex instead because of Enable/DisableAction now uses int for hashes.

### Fixed:
- Fixed head bob not working on time scale 0.
- Fixed some head bob issues with NaN values.
- Fixed graphics component not updating shadows properly.
- Fixed issue with only having a cinemachine camera assigned.
- Fixed jump height not changing with jump height multiplier.
- Fixed player not setting `IsRunning` to true when time scale is 0.
- Fixed player not being able to jump on low time scales.
- Fixed head bob throwing errors when time scale is 0.

---

## [1.6.0] - 2020-09-06

### Added:
- Added Gold Player Animator for easy and simple animation support.
- Added Gold Player Graphics that can be used to easily control the graphics visibility on the player.
- Added Gold Player Audio Animator (and Trigger) audio behaviour to allow animations to trigger audio events.
- Added Gold Player Object Bob that allows you to assign multiple objects to bob as the player moves.
- Added a custom editor for Gold Player Input System and Gold Player Input.
- Added support for disabling certain components using scripting define symbols and also through project settings (2018.3+).
- Added support for Cinemachine.
- Added support for both Text and TextMeshProUGUI on Gold Player UI, no matter the version. (But TMP is still 2018.1+!). **WILL BREAK YOUR UI WHEN UPGRADING!**
- Added new examples, both in Unity package and package manager version.
- Added support for jumping to require stamina.
- Added support for different stamina regen rates depending on if the player is moving or not.

### Improvements:
- When creating a player through 'GameObject - 3D - Gold Player Controller', it now fills in audio clips if the standard ones are present.
- Player Audio Behaviour can now handle audio by itself instead of it always being handled by Gold Player.
- Unity 2019.3+ is no longer a requirement for the new input system to function. As long as it's enabled and present, Gold Player will adapt to it.
- Now adapts classes better to uGUI and TextMeshPro.
- Improved code stripping. It now strips out all obsolete methods and properties to minimize code size.
- Added more safety checks and also make sure they get stripped out on release builds to improve performance.
- Several quality of life improvements.

### Changes:
- **(BREAKING CHANGE)** Minimum supported version raised to 2018.4. Gold Player will still probably work on version below but issues with versions below 2018.4 may not be fixed or take longer to fix.
- Made `MovementInput`, `SmoothedMovementInput`, `ShouldJump`, `ShouldRun`, and `ShouldCrouch` public to be able to control movement through code.
- Input actions are now directly referenced instead of just using the whole input actions asset.
- Changed folder structure to match the same structure of packages.
- Removed all sub-menus (except for example scripts) for Gold Player scripts in the 'Add Component' menu.
- (**BREAKING CHANGE**) Removed all sub-namespaces from scripts. All Gold Player scripts are now in `Hertzole.GoldPlayer`. Your code may break if you've referenced Gold Player in code but you mostly only need to remove old namespaces.
- Changed mouse sensitivity to a Vector2 instead to allow control over each axis' sensitivity.
- Changed wording on Run/Crouch Toggle modes. Off -> Hold, Permanent -> Toggle
- Changed wording on running modes. Faster Than Run Speed -> Is Running.
- Removed `GOLD_PLAYER_INTERACTION` script define. **It's now recommended you ONLY remove components using project settings/script defines**.

### Fixes:
- Fixed head bob strafe tilt still being applied even when the player can't move.
- Fixed mouse sensitivity being used twice when multiplying. **NOTE: May be a breaking change in user experience!**
- Fixed character center warning not updating properly.
- Fixed camera controls not working consistently depending on V-sync and timescale.
- Fixed bob targets flying off if their Z position wasn't 0.
- Fixed player not following along with really slow moving platforms.
- Fixed typo in GoldPlayerAudioBehaviour. PlayFoostepSound -> PlayFootstepSound
- Fixed the player continuing to run if running when CanRun is set to false.

---

## [1.5.1] - 2020-01-02
### Fixes:
- Fixed 'Can Move Around' not working with the normal input manager.

## [1.5.0] - 2020-01-01
### Added:
- Movement multipliers in code. Can be used to easily make the player faster/slower and jump higher.
- Camera force look.
- `SetPosition`, `SetLocalPosition`, and `SetPositionAndRotation` methods added to easily set the position.

### Improvements:
- Eliminated garbage caused by string concatenation.

### Changes:
- Removed all references to HertzLib update manager.

### Fixes:
- Fixed not being able to use the old input manager.

---

## [1.4.1] - 2019-12-20
### Improvements:
- Input now looks for an interface instead of a class. It makes it much easier to make your own input class.
- You can now manually control when input gets enabled and disabled in Gold Player Input System.
- Interaction now looks for an interface instead of just one class. It makes it much easier to make your own interactable objects through code.

### Fixes:
- Fixed using normal input manager not working.
- Fixed Null error with interaction component.

## [1.4.0] - 2019-11-06
### Added:
- Added support for Unity's new input system (requires 2019.3+ to work with Gold Player!)
- Added input strings in the editor, in the new "Input" tab.
- Added warnings in the editor when something isn't quite right.
- When using Unity 2019.1+ the editor GUI will be using the new UIElements.
- Added recoil support in player camera. (`Camera.ApplyRecoil(amount, time)`)
- Added support for UGUI now being a package in 2019.2+.
- Overhauled moving platforms. Now supports *all* moving objects and no longer reparents.

### Changes:
- Changed all formatting in variables from `m_FieldName` to `fieldName`.
- Cached Time.deltaTime to give a very, *very* slight performance improvement.
- Removed GroundCheckType property as the option was removed in 1.2.
- You're now required to have a GoldInput component on your player.
- Creating a player controller now adds a GoldInput component.

### Fixes:
- Fixed a problem with the max values in movement speeds.
- Fixed movement asking for input when the feature is disabled.
- Fixed the camera being FPS dependent. (Was very sluggish on high FPS and fast on lower FPS)
- Fixed errors that would occur when HertzLib Update Manager was present.
- Fixed errors appearing when playing and recompiling.
- Fixed the player stopping on a dime when Can Move Around was changed when moving.

---

## [1.3.0] - 2019-03-24
### Added:
- Added support for reference and auto find specific player components in UI
- Added full support for Unity 2018.3
- Added warning when character controller center doesn't match its height
- Added warning when the crouch height is too small
- Added warning when the player layer is included as a ground layer
- Added force support (knockback)
- Added support for creating a Gold Player Controller using Create GameObject content menu
- Added multiple jumps support (basically double jumping)
- Added run/crouch toggle options (thanks [@nickgravelyn](https://github.com/nickgravelyn)!)
- Added Unity package manager support (EXPERIMENTAL!)

### Changes:
- PlayerBehaviour no longer requires Gold Player Controller

### Bug fixes:
- Fixed UI throwing an error if there was no player referenced
- Fixed the crouch camera height being inverted
- Fixed problems with `MovementSpeeds.Max()`
- Fixed problems with nested interactables (thanks [@nickgravelyn](https://github.com/nickgravelyn)!)

---

## [1.2.3] - 2018-08-20
### Bug fixes:
- Fixed .NET 4.6 code in UI not working
- Fixed UI not auto finding player even if it was checked

## [1.2.2] - 2018-08-15
### Added:
- Added support for sprinting feature in Gold Player UI

### Bug fixes:
- Fixed Gold Player defines removing other defines.

## [1.2.1] - 2018-07-22
### Changes:
- UI component no longer has to be attached to the player

### Bug fixes:
- Fixed Interaction not updating correctly

## [1.2.0] - 2018-07-15
### Added:
- Added audio behaviour that hooks into the audio part, for more control over the audio
- Added player interactions; Easily add interactable elements in your world
- Added player UI; Hooks into interactions to show interaction messages
- Added assembly definitions

### Breaking changes:
- All `Init` methods have been renamed to `Initialized`
- Removed ground check type. Now only sphere is available

---

## [1.1.0] - 2018-01-09
### Added:
- Added camera shake
- Added player events
- Added box ground check
- Added air-jumping
- Added air control
- Added moving platform support

### Bug fixes:
- Fixed player Y rotation can't be edited in the editor of from a script
- Fixed the player stopping mid-air if `CanMove` changed while in the air
- Fixed typo in PlayerCamera
- Fixed camera still smoothing after `CanLook` has been disabled and then re-enabled

---

## [1.0.0] - 2017-12-25
### First release!