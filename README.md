# RAT Demo Project

The Real Accessibility Toolkit (RAT) is a videogame accessibility package for the Unity engine.

This repository contains the files for just the RAT as well as a demo project to display the toolkit's functions.

The demo project uses Unity's Survival Shooter tutorial project and **targets Unity LTS version 2019.4.12f1**.

A short video of the demo project can be watched here:

[<img src="https://i.ytimg.com/vi/BYrZ6uTeVuo/maxresdefault.jpg" width="50%">](https://www.youtube.com/watch?v=BYrZ6uTeVuo "RAT Demo Video")

## Usage guide
A more [in-depth ReadMe](RAT/RAT%20README.txt) can be found in the RAT files with instructions on how all the features work.

### Quick Setup
Go to folder RAT -> Settings -> Default Settings resource.
Enable or disable the accessibility modules you want.

Click the arrow to see sub-settings for each module.

- For outlines or highlighting, enable the effect(s) in the vision sub-setting as well as ticking the corresponding 'Auto Enable'.
  - Click the 'Low contrast identifiers' and/or 'Outline identifiers' dropdown depending on what you're using.
  - Add new elements to the list for what you want to target. Each identifier requires a tag and a layer they're on to find the target GameObjects.

- For subtitles, enable subtitles in the hearing sub-setting.
  - To show a subtitle, call `RAT_Controller.Instance.Hearing.ShowSubtitle()` with the text and time to show on screen.

- For directional captioning, enable captioning and click the Caption List dropdown.
  - Add new elements with IDs and the caption to display.
  - You'll need to set a player transform as the origin point first by calling `RAT_Controller.Instance.Hearing.SetPlayerTransform()` and pass the transform.
  - To show the caption, call `RAT_Controller.Instance.Hearing.ShowCaption()` with the caption ID.

- For menu scanning, enable the option in the motor sub-setting.
