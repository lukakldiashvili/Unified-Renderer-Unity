![UnifiedRenderer - Banner SVG](https://user-images.githubusercontent.com/42884387/115162333-fda57000-a0b3-11eb-9bf0-f4876e7bba8b.png)

<b>Stop creating new materials just to change color of the objects, modify them right from the component! (like with images)</b>

## Unified Renderer

Unified Renderer is simple drop-in plugin for Unity, which simplifies working with renderers and materials.<br><br>
Unified Renderer is based on Unity's 'MaterialPropertyBlock' class, that allows to pass different data to each Renderer/object.<br><br>
<i>By adding Unified Renderer to your project, you keep all of the built-in functionality, this package is just an extra layer which simplifies working with that functionality.</i><br><br>

## How to install
Simply add the following git url in package manager (for prototypes):<br>
```https://github.com/lukakldiashvili/Unified-Renderer-Unity.git```<br>

<b>For production, please find specific version in releases tab, and install latest package with url from there.</b>

## Features

- Not dependent on any SRP, so it is compatible with every one of them
- Set and Get float/int, color, vector and texture values per renderer
- Option to display renderer's and mesh filter's inspectors in one place, unified renderer component and hide others' contents
- Supports multiple materials (even the same instance) on single object

## Examples

![unified-renderer-demo](https://user-images.githubusercontent.com/42884387/119264266-763ba700-bbf3-11eb-8f1a-5fe32bd40a6b.gif)
#### <i> Note: each ball has the same default material assigned. this demo is included in the project. </i>

## Installation and Usage

- Install Unified Renderer by downloading .unitypackage file from release tab on github and opening it inside unity
- You can find unified renderer's settings inside project settings, under 'Unified Renderer' tab
- Add Unified Renderer component to object with supported renderer attached
- Add property you want to modify from the component
- Edit/View values from the inspector of from the scripts
