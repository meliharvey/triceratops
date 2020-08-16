# Example Website

This is a simple website that demonstrates how to load your 3d model as a three.js scene.

## Testing with the Example Website

You might be used to testing a static website simply by opening the html file in your browser. However, because three.js loads a JSON from JavaScript, your browser's CORS policy will likely reject the request. To get around this, there are 2 options below. I suggest using Option A, although you'll have to install python.

### Option A
For this option you must have python installed on your machine. If you don't have python installed already, follow this guide on [installing python](https://developer.mozilla.org/en-US/docs/Learn/Common_questions/set_up_a_local_testing_server) (if you have a Mac it has python installed natively).

#### Step 1
Once python is installed on your machine, open the Windows Command Prompt and cd into the folder at ```/Triceratops/examples/www```.

#### Step 2
Once in this directory type ```python3 -m http.server 8000``` into the Command Prompt and press enter.

...or if for some reason you're using Python 2 type ```python -m simpleHTTPServer 8000``` instead.

Press enter and python will start a basic web server and will print out something like:
```
Serving HTTP on 0.0.0.0 port 8000 (http://0.0.0.0:8000/) ...
```

#### Step 3
Open your favorite browser, and in the search bar type ```localhost:8000```. Press enter and your website should open!

#### Step 4
Whenever you export a new version of the 3d model from Triceratops, refresh the page to reload the latest version of your model.

### Option B
If you don't have python installed, you can disable your browsers web security. In this example I use Chrome, but other browsers might have a similar feature.

#### Step 1
First, open up your Windows Command Prompt by typing ```cmd``` into the search box on the Windows menu.

#### Step 2
Copy and paste the following into the command prompt by right clicking within the command prompt. You may need to change the path if your chome.exe file is in a different location.

```
"(x86)\Google\Chrome\Application\chrome.exe" --disable-web-security --user-data-dir=~/chromeTemp
```

Once this is copied in the command prompt, press enter on your keyboard. This is going to open a version of chrome that has web security disabled. Please DON'T open any websites other than the personal website that you're testing.

#### Step 3
Drag and drop the index.html into this new browser window.

#### Step 4
Whenever you export a new version of the 3d model from Triceratops, refresh the page to reload the latest version of your model.
