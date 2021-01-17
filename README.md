# Gold Player
A first-person player controller for Unity, built with being user-friendly and developer-friendly in mind.

## 🎇 Features

- Optimized for speed and size
- Walking, running (with optional stamina!), crouching, and jumping
- Head bob and audio for footsteps, jumping, and landing
- Moving platforms support
- Easy to use functions
- Very well documented code
- Interaction component, UI component, graphics component, and animator component
- Easy to integrate into multiplayer
- Support for [Unity's new Input System](http://docs.unity3d.com/Packages/com.unity.inputsystem@latest/)
- Works with IL2CPP and fast enter play mode settings

## 📦 Installation
**Minimum supported Unity version: 2018.4**

### Package manager installation:
1. Open the package manager in your project.
2. Click the + button in the top left corner and click 'Add package from git'.
3. Enter this URL: `https://github.com/Hertzole/gold-player.git#package`

or  

1. Open your `manifest.json` in `YourGameFolder/Packages/`.
2. Add `"se.hertzole.gold-player": "https://github.com/Hertzole/gold-player.git#package"` to your dependencies list.
3. Go back into Unity and it should resolve the packages and download Gold Player.

### Unity package installation:
1. Go to the [releases tab](https://github.com/Hertzole/gold-player/releases) and download the latest .unitypackage version.
2. In Unity, import the package either by dragging it into your project or right-clicking in your project window - Import package - Custom package
3. **Include all the files in the editor/runtime folder.** Components you don't want can be removed with script defines/project settings. You may remove examples if you want to.

### Development branch:
There's a development branch you can use to get the latest features in package manager format. **This branch may be unstable and features are not production ready!**  
In the package manager, enter  
`https://github.com/Hertzole/gold-player.git#dev-package` instead.  
In your manifest, enter  
`"se.hertzole.gold-player": "https://github.com/Hertzole/gold-player.git#dev-package"` instead.

### Updating:
**Package manager version:** Remove the package and add it again.  
**Unity package version:** Remove the old project and reimport it.

## 🔨 Getting Started
After importing Gold Player into your project, the quickest way to get started is by creating the player using Create - 3D Object - Gold Player Controller. This will set up the player in a recommended way.  
You can then explore all the options on the controller. You can also check out all the available components from the Add Component menu. They are under the Gold Player subcategory. You can also check out the examples that demonstrate specific features.

## 📃 License
Gold Player is licensed under MIT. You can do whatever you want with it, but I'm not liable if it causes any damages.

## 🕹 Screenshots and Testing
**[Play Example Level](https://hertzole.github.io/gold-player/docs/play)**  
You can play around in an example scene that showcases most of the features while also allowing you to modify the player at runtime. 

The available editors:
![Editor](https://raw.githubusercontent.com/Hertzole/gold-player/gh-pages/docs/screenshots/editor.png)

##

<a href='https://ko-fi.com/I2I4IHAK' target='_blank'><img height='40' style='border:0px;height:40px;' src='https://help.ko-fi.com/hc/article_attachments/360016971454/Ko-fi_Red.png' border='0' alt='Buy Me a Coffee at ko-fi.com' /></a> 
