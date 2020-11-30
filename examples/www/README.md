# Example Website

This is a simple website that demonstrates how to load your 3d model as a three.js scene.

## Swapping out models

By default, the example site loads ```chair.json```. To update this to open a new model open Triceratops/examples/app.js and replace ```chair.json``` with the name of the json file you would like to load.
```
loader.load(
  // resource URL
  "./assets/[yourModelFile].json",
```

## Testing with the Example Website

You might be used to testing a simple static website simply by opening the html file in your browser. However, because three.js loads a JSON from JavaScript, your browser's CORS policy will likely reject the request. You will need to launch a server using the following steps.

### For Windows
#### Step 1
Search for Rhinoceros in the Windows search bar. When you see the Rhinoceros icon right click on it and select "Run as administrator".

#### Step 2
Launch grasshopper and then navigate to Triceratops > File Management. Drag the HTTPServer ![HTTPServer](assets/icons/Tri_HTTPServer.png) component onto the Grasshopper canvas.

#### Step 3
The ```Path(P)``` input should be a string of the path to the directory where your index.html is located. The ```Run(R)``` should take a boolean toggle as an input.

#### Step 4
Toggle the boolean toggle to true and the server should start running and open a new tab in your browser with the website you're serving.

#### Step 5
Whenever you export a new version of the 3d model from Triceratops, refresh the page to reload the latest version of your model.


### For Mac
#### Step 1
Use finder to navigate to your websites root folder (i.e. where index.html is located). For the Triceratops example files this is ```../Triceratops/examples/www```. Right click on the folder and click ```New Terminal at folder``` to open the terminal in this location.

#### Step 2
In the terminal, type ```python3 -m http.server 8000``` into the terminal and press enter (or if you're using python 2 for some reason ```python -m simpleHTTPServer 8000```).

Python will launch a server and give you a message like this:
```
Serving HTTP on 0.0.0.0 port 8000 (http://0.0.0.0:8000/) ...
```

#### Step 3
Open your favorite browser and in the search bar type ```localhost:8000```. Press enter and your website should open.

#### Step 4
Whenever you export a new version of the 3d model from Triceratops, refresh the page to reload the latest version of your model.
