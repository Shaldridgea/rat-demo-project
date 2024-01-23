RAT - Real Accessibility Toolkit

--------- Usage Guide ---------

1. Quick setup
2. Overview
3. Vision
4. Hearing
5. Motor

--------- 1. Quick setup ---------

Go to folder RAT -> Settings -> Default Settings resource.
Enable or disable the accessibility modules you want.

Click the arrow to see sub-settings for each module.

- For outlines or highlighting, enable the effect(s) in the vision sub-setting as well as ticking the corresponding 'Auto Enable'.
	- Click the 'Low contrast identifiers' and/or 'Outline identifiers'
	dropdown depending on what you're using.
	- Add new elements to the list for what you want to target. Each identifier requires
	a tag and a layer they're on to find the target GameObjects.

- For subtitles, enable subtitles in the hearing sub-setting.
	- To show a subtitle, call RAT_Controller.Instance.Hearing.ShowSubtitle() with the text
	and time to show on screen.

- For directional captioning, enable captioning and click the Caption List dropdown.
	- Add new elements with IDs and the caption to display.
	- You'll need to set a player transform as the origin point first
	by calling RAT_Controller.Instance.Hearing.SetPlayerTransform() and pass the transform.
	- To show the caption, call RAT_Controller.Instance.Hearing.ShowCaption() with the caption ID.

- For menu scanning, enable the option in the motor sub-setting.


--------- 2. Overview ---------

The RAT provides several pre-made utilities designed with the aim of being simple to setup
and get started using in your project.

This toolkit targets some common accessibility issues for visually-impaired players,
deaf and hard of hearing players, and players with motor control impairments.

After importing the package, you'll want to go to the RAT folder and into Settings.
There you'll see a Default Settings resource, which is what you'll use to configure RAT
for your project.

If you have vision, hearing, or motor accessibility enabled on this top level resource,
there will be sub-settings for the individual settings you can view by clicking the arrow
and selecting them.

If you require to have multiple settings for different scenarios, you can create extra
RAT Settings through the Create Asset menu like regular assets. Only one of these settings
can be marked as default for startup. The current settings in-use can be changed at runtime
by calling RAT_Settings.SetSettingsInstance() and passing the name e.g. the default is
simply called "Default".


--------- 3. Vision ---------

The vision module provides three features aimed at alleviating issues
vision-impaired players may face while playing your game.

Colorblind correction, the simplest, targets one of the three types of colorblindness
and uses a filter to adjust the colors on-screen to make the game visually clearer for
those players.

Outlines do as you might expect and add colored outlines around the targeted GameObjects
to make certain gameplay elements more visually distinct for players.

Low contrast mode, first of all, creates an effect over the entire screen that renders it in
greyscale, along with edge detection to make the edge of objects clearer. Secondly,
you can define highlights to show certain gameplay elements in a solid block of color.
Together these can help players whose vision struggles to distinguish detail.

For both outlines and low contrast highlights, there is a list of identifiers you can edit to
configure how the outlines and highlights are displayed. For both of them, an identifier is made
up of a name, a tag, a Layer, and a color. The tag and layer are used together to find and
target the specific GameObjects in the scene you want to change the appearance of. The color
is what is displayed by the specific feature. It is generally advisable to use very bright,
distinct colours for outlines and highlights to ensure they are as visually understandable and
different as possible, so as to separate them from less important elements in the scene.

For outlines and highlights as well, while you can use the Auto Enable flags to automatically
target objects in the scene that match any identifier, you'll need to add components manually to
newly instantiated objects at runtime. You can do this by adding either a LowContrastHighlight
component or an Outline component to the prefab. The LowContrastHighlight can be
placed on the root GameObject of an entity, but the Outline should be placed on the actual
GameObject that has the mesh renderer if that is not the root object.


--------- 4. Hearing ---------

The hearing module provides two features to help deaf and hard of hearing players
while playing your game.

Subtitling gives an easy way to get any dialogue in your game up on the screen.
Pass the text and how long it should be on screen to
RAT_Controller.Instance.Hearing.ShowSubtitle() to make it appear on screen.
You can also enable animating for subtitles, which will make the text scroll on screen
with a typewriter effect, and the subtitle boxes will fade away when done.

Subtitles can also identify a speaker by color, using the speaker identifiers and using the name
delimiter. The default color is what is used for subtitles with no speaker associated. The delimiter
is a character like a colon : that separates the speaker name from what they're saying in the text
that is passed. It then will automatically use that to separate existing speaker names and match
that with any speaker identifiers. Identifiers have a name for matching, and the color that
will be used for their dialogue.

Captioning lets you inform the player of important sound effects that have played,
and where they're coming from. This can be vital to important gameplay to inform deaf/HoH
players that they're being damaged, that something is impassable or dangerous, or other important
sound cues. Captions will appear in the bottom right and fill up the right side of the screen
if there are multiple of them. The hearing display prefab can be edited if you wish to change
how much space it takes up on-screen or its scaling.

You'll need to define captions before using them. In the hearing sub-setting, click
the Caption List dropdown to add new captions. Each caption has an id, caption text, and a priority.
To show a caption, call RAT_Controller.Instance.Hearing.ShowCaption(), passing the id of the
caption you want to display, along with the length of time the sound plays, and either the transform
or a position associated with the sound for the caption to point to.
The priority of a caption indicates its importance. Higher priority values will put
and keep high priority captions in the corner, where the player will more easily and
instantly see them. This can be useful to avoid less important sound cues drowning out
more important ones, as only so many can be shown on-screen at once.

For directional captioning, you'll also need to set a player transform as the
origin point first by calling RAT_Controller.Instance.Hearing.SetPlayerTransform()
and passing the transform.


--------- 5. Motor ---------

The motor module only has one feature to help players with motor control impairments,
due to the very specific nature of these issues that can vary a lot from game to game.

Menu scanning lets the game scroll through a list of menu options automatically, waiting on
each UI element to let the player choose what they want to interact with without lots of
specific or repetitive movements to navigate between the menu items manually.

After turning this on, you'll need to set the starting menu item in any menu that is presented
to the player by calling RAT_Controller.Instance.Motor.SetMenuScanningSelectable(). The module
will then automatically cycle through selectable UI elements in a clockwise fashion.