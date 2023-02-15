![UnifiedRenderer - Banner SVG](https://user-images.githubusercontent.com/42884387/115162333-fda57000-a0b3-11eb-9bf0-f4876e7bba8b.png)

<b>Stop creating new materials just to change color of the objects, modify them right from the component!</b>

## Unified Renderer

Unified Renderer is simple plugin for Unity, that allows to assign material properties right from object (instead of material).<br><br>
Unified Renderer is based on Unity's 'MaterialPropertyBlock' class, that allows to pass different data to each Renderer/object.<br><br>

<p align="center">
  <img width="700" align="center" alt="demo" src="https://user-images.githubusercontent.com/42884387/219176026-5a5546ab-feca-4dfc-9b68-77acdecebc74.gif">
</p>


## How to install
Simply add the following git url in package manager (please read further notes):<br>
```https://github.com/lukakldiashvili/Unified-Renderer-Unity.git```<br>

<b>For production, please find specific version in releases tab, and install latest package with url from there.</b>

## Features

- SRP independent, works with all render pipelines.
- Set/Get property values per renderer
- Option to display renderer's and mesh filter's inspectors in one place, unified renderer component and hide others' contents
- Per-material or per-renderer (global for renderer) properties

## Examples

![unified-renderer-demo](https://user-images.githubusercontent.com/42884387/119264266-763ba700-bbf3-11eb-8f1a-5fe32bd40a6b.gif)
#### <i> Note: each ball has the same default material assigned. this demo is included in the project. </i>

## Installation and Usage

- Install Unified Renderer as upm package (how to: https://docs.unity3d.com/Manual/upm-git.html)
 <br> or install manually by putting repository into the project
- You can find settings inside project settings, under 'Unified Renderer' tab
- Add Unified Renderer component to object with supported renderer attached
- Add property you want to modify from the component
- Edit/View values from the inspector of from the scripts
