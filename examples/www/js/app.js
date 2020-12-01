/**
 * @summary Simple shell website for Triceratops scene
 * @author Meli <meliharvey87@gmail.com>
 */

var camera, controls, scene, renderer;

console.log(window.location.href)

init();
animate();

function init() {

  scene = new THREE.Scene();

  // get the container element from the DOM
  container = document.getElementById('container');

  // create the rendered and set it to the height/width of the container
  renderer = new THREE.WebGLRenderer( { antialias: true } );
  renderer.setSize(container.clientWidth, container.clientHeight);
  renderer.shadowMap.enabled = true; // if you don't want shadows, set to false
  renderer.setClearColor (0xeeeeee, 1); // this is the background color seen while scene is loading
  container.appendChild( renderer.domElement );

  // create camera (default field of view is 60)
  camera = new THREE.PerspectiveCamera( 60, container.clientWidth / container.clientHeight, 0.1, 10000 );
  camera.position.set( 40, 40, 40 ); // starting position of the camera

  // camera controls to allow for orbiting
  controls = new THREE.OrbitControls( camera, renderer.domElement );
  controls.enableDamping = true; // creates a softer orbiting feel
  controls.dampingFactor = 0.1; // determines how soft
  controls.screenSpacePanning = true;
  controls.maxPolarAngle = Math.PI / 2;

  // this is only required when using RectAreaLight
  THREE.RectAreaLightUniformsLib.init();

  // load scene
  var loader = new THREE.ObjectLoader();

  loader.load(
  	// resource URL
  	"./assets/house.json",

  	// onLoad callback
  	function ( obj ) {
      // remove the loading text
      document.getElementById('progress').remove();

  		// assign the loaded object to the scene variable
  		scene = obj;
  	},

  	// onProgress callback
  	function ( xhr ) {
      progressText( xhr ) // delete this if you don't want the progress text
  	},

  	// onError callback
  	function ( err ) {
  		console.error( 'An error happened' );
  	}
  );

  // listen for changes to the window size to update the canvas
  window.addEventListener( 'resize', onWindowResize, false );

}

// adds progress text while the model is loading
function progressText( xhr ) {
  var progress, textNode, text;

  if (document.getElementById('progress')) {
    document.getElementById('progress').remove();
  }

  if (xhr.lengthComputable) {
    text = 'loading: ' + Math.round((xhr.loaded / xhr.total * 100)) + '%'
  } else {
    text = 'loading: ' + Math.round(xhr.loaded / 1000) + 'kb'
  }

  console.log(text);

  progress = document.createElement('DIV');
  progress.id = 'progress';
  textNode = document.createTextNode(text);
  progress.appendChild(textNode)
  container.appendChild(progress)
}

// function for handling resize events
function onWindowResize() {

  camera.aspect = container.clientWidth / container.clientHeight;
  camera.updateProjectionMatrix();

  renderer.setSize( container.clientWidth, container.clientHeight );

}

// animates the scene
function animate() {

  requestAnimationFrame( animate );

  controls.update();
  render();

}

function render() {

  renderer.render( scene, camera );

}
