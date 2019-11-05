## 1.4
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

## 1.3
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

## 1.2.3
### Bug fixes:
- Fixed .NET 4.6 code in UI not working
- Fixed UI not auto finding player even if it was checked

## 1.2.2
### Added:
- Added support for sprinting feature in Gold Player UI

### Bug fixes:
- Fixed Gold Player defines removing other defines.

## 1.2.1
### Changes:
- UI component no longer has to be attached to the player

### Bug fixes:
- Fixed Interaction not updating correctly

## 1.2.0
### Added:
- Added audio behaviour that hooks into the audio part, for more control over the audio
- Added player interactions; Easily add interactable elements in your world
- Added player UI; Hooks into interactions to show interaction messages
- Added assembly definitions

### Breaking changes:
- All `Init` methods have been renamed to `Initialized`
- Removed ground check type. Now only sphere is available

---

## 1.1.0
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

## 1.0.0
### First release!