var camera, controls, scene, renderer;

init();
//render(); // remove when using next line for animation loop (requestAnimationFrame)
animate();

function init() {

  scene = new THREE.Scene();

  container = document.getElementById('container');

  renderer = new THREE.WebGLRenderer( { antialias: true } );
  renderer.setSize(container.clientWidth, container.clientHeight);
  container.appendChild( renderer.domElement );

  camera = new THREE.PerspectiveCamera( 60, window.innerWidth / window.innerHeight, 1, 1000 );
  camera.position.set( 20, 20, 20 );

  // controls
  controls = new THREE.OrbitControls( camera, renderer.domElement );
  controls.enableDamping = true;
  controls.dampingFactor = 0.1;
  controls.screenSpacePanning = false;
  controls.maxPolarAngle = Math.PI / 2;

  // this is required when using RectAreaLight
  THREE.RectAreaLightUniformsLib.init();

  // load scene
  var loader = new THREE.ObjectLoader();

  loader.load(
  	// resource URL
  	"../assets/example_scene.json",

  	// onLoad callback
  	// Here the loaded data is assumed to be an object
  	function ( obj ) {
  		// Add the loaded object to the scene
  		scene = obj;
  	},

  	// onProgress callback
  	function ( xhr ) {
  		console.log( (xhr.loaded / xhr.total * 100) + '% loaded' );
  	},

  	// onError callback
  	function ( err ) {
  		console.error( 'An error happened' );
  	}
  );

  window.addEventListener( 'resize', onWindowResize, false );

}

function onWindowResize() {

  camera.aspect = container.clientWidth / container.clientHeight;
  camera.updateProjectionMatrix();

  renderer.setSize( container.clientWidth, container.clientHeight );

}

function animate() {

  requestAnimationFrame( animate );

  controls.update();
  render();

}

function render() {

  renderer.render( scene, camera );

}
