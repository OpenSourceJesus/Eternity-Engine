using TMPro;
using System;
using System.IO;
using Extensions;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Diagnostics;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EternityEngine
{
	public class EternityEngine : SingletonUpdateWhileEnabled<EternityEngine>
	{
		public _Object obPrefab;
		public HierarchyEntry hierarchyEntryPrefab;
		public Image insertionIndicatorPrefab;
		public _Component[] componentsPrefabs = new _Component[0];
		public GameObject  onlyOneComponentPerObjectAllowedNotificationGo;
		public TMP_Text  onlyOneComponentPerObjectAllowedNotificationText;
		public GameObject  cantDeleteComponentNotificationGo;
		public TMP_Text  cantDeleteComponentNotificationText;
		public RectTransform canvasRectTrs;
		public ColorValue backgroundColor;
		public BoolValue useGravity;
		public Vector3Value gravity;
		public FloatValue unitLen;
		static _Object[] obs = new _Object[0];
		static bool prevDoDuplicate;
		static bool prevSelectAll;
		static string BUILD_SCRIPT_PATH = Path.Combine(Application.dataPath, "Others", "Python", "Build.py");
		static StreamReader errorReader;
		static Dictionary<string, string> attributes = new Dictionary<string, string>();
		static string apiCode;
		static string initCode;
		static string[] updateScripts = new string[0];
		static string[] vars = new string[0];
		static string[] uiMethods = new string[0];
		static string[] renderCode = new string[0];
		static string[] particleSystemsClauses = new string[0];
		static string[] uiClauses = new string[0];
		static string[] globals = new string[0];
		static Dictionary<string, float[]> pivots = new Dictionary<string, float[]>();
		static string PYTHON = @"from python import os, sys, math, pygame, random, PyRapier2d
from random import uniform

os.environ['SDL_JOYSTICK_ALLOW_BACKGROUND_EVENTS'] = '1'

# Physics Section Start
sim = PyRapier2d.Simulation()
# rigidBodiesIds = {}
# collidersIds = {}
rigidBodiesIds = {'' : (-1, -1)}
rigidBodiesIds.clear()
collidersIds = {'' : (-1, -1)}
collidersIds.clear()
jointsIds = {}
# Physics Section End
surfaces = {'' : pygame.Surface((0, 0))}
surfaces.clear()
hide = []
surfacesRects = {'' : pygame.Rect(0, 0, 0, 0)}
surfacesRects.clear()
initRots = {'' : 0.0}
initRots.clear()
zOrders = {'' : 0.0}
zOrders.clear()
if sys.platform == 'win32':
	TMP_DIR = os.path.expanduser('~\\AppData\\Local\\Temp')
else:
	TMP_DIR = '/tmp'
pivots = {}
# Attributes
mousePos = pygame.math.Vector2()
mousePosWorld = pygame.math.Vector2()
prevMousePos = pygame.math.Vector2()
prevMousePosWorld = pygame.math.Vector2()
off = pygame.math.Vector2()
sortedObNames : list[str] = []

def add (v, v2):
	return pygame.math.Vector2(v[0] + v2[0], v[1] + v2[1])

def subtract (v, v2):
	return pygame.math.Vector2(v[0] - v2[0], v[1] - v2[1])

def multiply (v, f):
	return pygame.math.Vector2(v[0] * f, v[1] * f)

def divide (v, f):
	return pygame.math.Vector2(v[0] / f, v[1] / f)

def magnitude (v) -> float:
	return math.sqrt(sqr_magnitude(v))

def sqr_magnitude (v) -> float:
	return v[0] * v[0] + v[1] * v[1]

def normalize (v):
	return divide(v, magnitude(v))

def copy_object (name, newName, pos, rot = 0, wakeUp = True, attachTo : str = '', copyParticles = True):
	global pivots, initRots, surfaces, surfacesRects, collidersIds, rigidBodiesIds
	surface = surfaces.get(name, None)
	if surface is not None:
		surface = surface.copy()
		surfacesRects[newName] = surfacesRects[name].copy()
		surfaces[newName] = surface
		pivots[newName] = pivots[name].copy()
		initRots[newName] = rot
		sortedObNames.append(newName)
	if name in attributes:
		attributes[newName] = attributes[name].copy()
	if name in zOrders:
		zOrders[newName] = zOrders[name]
	if name in rigidBodiesIds:
		newRigidBody = sim.copy_rigid_body(rigidBodiesIds[name], pos, 0, wakeUp)
		if newRigidBody is not None:
			rigidBodiesIds[newName] = newRigidBody
			for i, collider in enumerate(sim.get_rigid_body_colliders(newRigidBody)):
				collidersIds[newName + ':' + str(i)] = collider
	else:
		if surface is not None:
			surfaces[newName] = pygame.transform.rotate(surface, rot)
			initRots[newName] = rot
		if name in collidersIds:
			if attachTo == '':
				newCollider = sim.copy_collider(collidersIds[name], pos, rot)
			else:
				newCollider = sim.copy_collider(collidersIds[name], pos, rot, rigidBodiesIds[attachTo])
			if newCollider is not None:
				collidersIds[newName] = newCollider
	if name in particleSystems:
		particleSystem = particleSystems[name]
		newParticleSystem = particleSystem.copy(newName)
		if copyParticles:
			for particle in particleSystem.particles:
				particlePos = get_object_position(particle.name)
				particleRot = get_object_rotation(particle.name)
				newParticleName = newName + ':' + str(particleSystem.lastId)
				copy_object (particle.name, newParticleName, rotate_vector(particlePos, pos, rot), particleRot + rot)
				newParticleSystem.particles.append(Particle(newParticleName, particle.life))
		particleSystems[newName] = newParticleSystem

def remove_object (name, removeColliders = True, wakeUp = True, removeParticles = True):
	global sortedObNames
	if name in pivots:
		del surfaces[name]
		del surfacesRects[name]
		del initRots[name]
		del pivots[name]
		sortedObNames = [item for item in sortedObNames if item != name]
	if name in attributes:
		del attributes[name]
	if name in zOrders:
		del zOrders[name]
	if name in rigidBodiesIds:
		rigidBody = rigidBodiesIds[name]
		if removeColliders:
			for removeCollider in sim.get_rigid_body_colliders(rigidBody):
				for colliderName, collider in list(collidersIds.items()):
					if collider == removeCollider:
						del collidersIds[colliderName]
						break
		sim.remove_rigid_body (rigidBody, removeColliders)
		del rigidBodiesIds[name]
	elif name in collidersIds:
		sim.remove_collider (collidersIds[name], wakeUp)
		del collidersIds[name]
	if removeParticles and name in particleSystems:
		particleSystem = particleSystems.pop(name)
		for particle in particleSystem.particles:
			remove_object (particle.name)

def ang_to_dir (ang):
	ang = math.radians(ang)
	return pygame.math.Vector2(math.cos(ang), math.sin(ang))

def rotate_surface (surface, deg, pivot, offset):
	rotatedSurface = pygame.transform.rotate(surface, -deg)
	rotatedOff = offset.rotate(deg)
	rect = rotatedSurface.get_rect(center = pivot - rotatedOff)
	return rotatedSurface, rect

def rotate_vector (v, pivot, deg):
	deg = math.radians(deg)
	ang = math.atan2(v[1] - pivot[1], v[0] - pivot[0]) + deg
	return pivot + (pygame.math.Vector2(math.cos(ang), math.sin(ang)).normalize() * (pygame.math.Vector2(v) - pivot).length())

def degrees (ang):
	return float(math.degrees(ang))

def radians (ang):
	return float(math.radians(ang))

def get_object_position (name):
	if name in rigidBodiesIds:
		return sim.get_rigid_body_position(rigidBodiesIds[name])
	elif name in collidersIds:
		return sim.get_collider_position(collidersIds[name])
	else:
		raise ValueError('name needs to refer to a rigid body or a collider found in rigidBodiesIds or collidersIds')

def get_object_rotation (name):
	if name in rigidBodiesIds:
		return sim.get_rigid_body_rotation(rigidBodiesIds[name])
	elif name in collidersIds:
		return sim.get_collider_rotation(collidersIds[name])
	else:
		raise ValueError('name needs to refer to a rigid body or a collider found in rigidBodiesIds or collidersIds')

class Particle:
	name : str
	life : float

	def __init__ (self, name : str, life : float):
		self.name = name
		self.life = life

	def __eq__ (self, other : Particle) -> bool:
		if not isinstance(other, Particle):
			return False
		return self.name == other.name

class ParticleSystem:
	name : str
	particleName : str
	enable : bool
	prewarmDur : float
	minRate : float
	maxRate : float
	bursts : list[tuple[float, int]]
	currBurstIdx : int
	intvl : float
	minLife : float
	maxLife : float
	minSpeed : float
	maxSpeed : float
	minRot : float
	maxRot : float
	minSize : float
	maxSize : float
	minGravityScale : float
	maxGravityScale : float
	minBounciness : float
	maxBounciness : float
	maxEmitRadiusNormalized : float
	minEmitRadiusNormalized : float
	minLinearDrag : float
	maxLinearDrag : float
	minAngDrag : float
	maxAngDrag : float
	tint : list[float]
	shapeType : int
	shapeRot : float
	ballRadius : float
	timer : float
	lastId : int
	particles : list[Particle]

	def __init__ (self, name : str, particleName : str, enable : bool, prewarmDur : float, minRate : float, maxRate : float, bursts : list[tuple[float, int]], minLife : float, maxLife : float, minSpeed : float, maxSpeed : float, minRot : float, maxRot : float, minSize : float, maxSize : float, minGravityScale : float, maxGravityScale : float, minBounciness : float, maxBounciness : float, maxEmitRadiusNormalized : float, minEmitRadiusNormalized : float, minLinearDrag : float, maxLinearDrag : float, minAngDrag : float, maxAngDrag : float, tint : list[float], shapeType : int, shapeRot : float, ballRadius : float = 0.0):
		self.name = name
		self.particleName = particleName
		self.enable = enable
		self.minRate = minRate
		self.maxRate = maxRate
		self.intvl = 1.0 / uniform(minRate, maxRate)
		self.bursts = bursts
		self.currBurstIdx = 0
		self.minSize = minSize
		self.maxSize = maxSize
		self.minLife = minLife
		self.maxLife = maxLife
		self.minSpeed = minSpeed
		self.maxSpeed = maxSpeed
		self.minRot = minRot
		self.maxRot = maxRot
		self.minGravityScale = minGravityScale
		self.maxGravityScale = maxGravityScale
		self.minBounciness = minBounciness
		self.maxBounciness = maxBounciness
		self.maxEmitRadiusNormalized = maxEmitRadiusNormalized
		self.minEmitRadiusNormalized = minEmitRadiusNormalized
		self.minLinearDrag = minLinearDrag
		self.maxLinearDrag = maxLinearDrag
		self.minAngDrag = minAngDrag
		self.maxAngDrag = maxAngDrag
		self.tint = tint
		self.shapeType = shapeType
		self.shapeRot = math.radians(shapeRot)
		self.ballRadius = ballRadius
		self.timer = 0.0
		self.lastId = 0
		self.particles = []

	def update (self, dt : float):
		self.timer += dt
		if self.currBurstIdx < len(self.bursts):
			burst = self.bursts[self.currBurstIdx]
			if self.timer >= burst[0]:
				self.timer -= burst[0]
				for i in range(burst[1]):
					self.emit ()
				self.currBurstIdx += 1
		if self.timer >= self.intvl:
			self.timer -= self.intvl
			self.intvl = 1.0 / uniform(self.minRate, self.maxRate)
			self.emit ()
		for particle in list(self.particles):
			particle.life -= dt
			if particle.life <= 0:
				remove_object (particle.name)
				self.particles.remove(particle)

	def emit (self):
		newParticleName = self.name + ':' + str(self.lastId)
		self.lastId += 1
		obPos = get_object_position(self.name)
		rot = uniform(self.minRot, self.maxRot)
		normalizedRadius = uniform(self.minEmitRadiusNormalized, self.maxEmitRadiusNormalized)
		randRad = uniform(0, 2 * math.pi)
		if self.shapeType == 0: # ball
			pos = pygame.math.Vector2(obPos[0] + self.ballRadius * normalizedRadius * math.cos(randRad), obPos[1] + self.ballRadius * normalizedRadius * math.sin(randRad))
		else:
			pos = pygame.math.Vector2(0, 0)
		copy_object (self.particleName, newParticleName, pos, rot)
		surface = surfaces.get(newParticleName, None)
		if surface is not None:
			size = uniform(self.minSize, self.maxSize)
			surface = pygame.transform.scale_by(surface, size)
			surfaces[newParticleName] = surface
		rigidBody = rigidBodiesIds[newParticleName]
		for collider in sim.get_rigid_body_colliders(rigidBody):
			sim.set_bounciness (collider, uniform(self.minBounciness, self.maxBounciness))
			sim.set_collider_enabled (collider, True)
		sim.set_gravity_scale (rigidBody, uniform(self.minGravityScale, self.maxGravityScale), False)
		sim.set_linear_drag (rigidBody, uniform(self.minLinearDrag, self.maxLinearDrag))
		sim.set_angular_drag (rigidBody, uniform(self.minAngDrag, self.maxAngDrag))
		sim.set_rigid_body_enabled (rigidBody, True)
		sim.set_linear_velocity (rigidBody, ang_to_dir(math.degrees(randRad)) * uniform(self.minSpeed, self.maxSpeed))
		self.particles.append(Particle(newParticleName, uniform(self.minLife, self.maxLife)))

	def set_enabled (self, enable : bool):
		if enable and not self.enable:
			self.timer = 0.0
			self.intvl = 1.0 / uniform(minRate, maxRate)
			while self.timer < self.prewarmDur:
				self.timer += self.intvl
				self.update (self.intvl)
				self.intvl = 1.0 / uniform(self.minRate, self.maxRate)
			self.update (self.prewarmDur - self.timer)
		self.enable = enable

	def copy (self, newName : str):
		return ParticleSystem(newName, self.particleName, self.enable, self.prewarmDur, self.minRate, self.maxRate, self.bursts, self.minLife, self.maxLife, self.minSpeed, self.maxSpeed, self.minRot, self.maxRot, self.minSize, self.maxSize, self.minGravityScale, self.maxGravityScale, self.minBounciness, self.maxBounciness, self.maxEmitRadiusNormalized, self.minEmitRadiusNormalized, self.minLinearDrag, self.maxLinearDrag, self.minAngDrag, self.maxAngDrag, self.tint, self.shapeType, self.shapeRot, self.ballRadius)

particleSystems : dict[str, ParticleSystem] = {}

def handle_events ():
	for event in pygame.event.get():
		if event.type == pygame.QUIT:
			running = False

def init ():
	global ui, sim, off, hide, pivots, initRots, surfaces, jointsIds, attributes, uiCallbacks, collidersIds, surfacesRects, sortedObNames, rigidBodiesIds, particleSystems
# Globals
	sim = PyRapier2d.Simulation()
	rigidBodiesIds.clear()
	collidersIds.clear()
	jointsIds.clear()
	surfaces.clear()
	hide = []
	surfacesRects.clear()
	initRots.clear()
	particleSystems.clear()
	zOrders.clear()
	off = pygame.math.Vector2()
	ui.clear()
	uiCallbacks.clear()
# Init Pivots, Attributes, UI
# Init Physics
# Init Rendering
# Init Particle Systems
	sortedObNames = [name for name, z in sorted(zOrders.items(), key = lambda item : item[1])]

def update ():
	global off, mousePos, uiCallbacks, mousePosWorld, prevMousePos, prevMousePosWorld
# Globals
	mousePos = pygame.mouse.get_pos()
	mousePosWorld = mousePos + off
# Physics Section Start
	sim.step ()
# Physics Section End
# Update
	for particleSystem in list(particleSystems.values()):
		if particleSystem.enable:
			particleSystem.update (dt)
	for name, uiOb in list(ui.items()):
		if uiOb.enable:
			uiOb.update ()
	prevMousePos = mousePos
	prevMousePosWorld = mousePosWorld

def render ():
# Background
	for name in sortedObNames:
		if name not in hide:
			surface = surfaces[name]
			if name in rigidBodiesIds:
				rigidBody = rigidBodiesIds[name]
				pos = sim.get_rigid_body_position(rigidBody)
				rot = sim.get_rigid_body_rotation(rigidBody)
				width, height = surface.get_size()
				pivot = pivots[name]
				offset = pygame.math.Vector2(pivot[0] * width, pivot[1] * height) - pygame.math.Vector2(width, height) / 2
				rotatedSurface, rect = rotate_surface(surface, rot + initRots[name], pos, offset)
				screen.blit(rotatedSurface, (rect.left - off.x, rect.top - off.y))
			else:
				pos = surfacesRects[name].topleft
				screen.blit(surface, (pos[0] - off.x, pos[1] - off.y))
	pygame.display.flip()

# Vars
pygame.init()
screen = pygame.display.set_mode(flags = pygame.FULLSCREEN)
windowSize = pygame.display.get_window_size()
pygame.display.set_caption('Game')
clock = pygame.time.Clock()
running = True
dt = clock.tick(60) / 1000
frame = 0

class UIElement:
	name : str
	enable : bool

	def __init__ (self, name : str, enable : bool):
		self.name = name
		self.enable = enable

	def update (self):
		if self.name in uiCallbacks and surfacesRects[self.name].collidepoint(mousePosWorld) and not surfacesRects[self.name].collidepoint(prevMousePosWorld):
			uiCallbacks[self.name] ()

	def copy (self, newName : str, enable : bool):
		newUIElt = UIElement(newName, enable)
		uiCallbacks[newName] = uiCallbacks[self.name].copy()
		return newUIElt

ui : dict[str, UIElement] = {}
uiCallbacks : dict[str, Callable[[], None]] = {}
# API
# UI Methods
init ()
# Init User Code
while running:
	dt = clock.tick(60) / 1000
	handle_events ()
	update ()
	render ()
	frame += 1
";

#if UNTIY_EDITOR
		public override void Awake ()
		{
			base.Awake ();
			obs = new _Object[0];
		}
#endif

		void OnDestroy ()
		{
			if (errorReader != null)
				errorReader.Close();
		}

		public override void DoUpdate ()
		{
			bool leftCtrlKeyPressed = Keyboard.current.leftCtrlKey.isPressed;
			HierarchyPanel firstHierarchyPanel = HierarchyPanel.instances[0];
			bool upArrowPressed = Keyboard.current.upArrowKey.wasPressedThisFrame;
			bool downArrowPressed = Keyboard.current.downArrowKey.wasPressedThisFrame;
			if ((upArrowPressed || downArrowPressed) && firstHierarchyPanel.selected.Length == 0)
			{
				for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
				{
					HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
					hierarchyPanel.entries[0].OnMouseDown ();
				}
			}
			else if (upArrowPressed && HierarchyPanel.lastEntryIdxHadSelectionSet > 0)
				DoArrowKeySelection (true);
			else if (downArrowPressed && HierarchyPanel.lastEntryIdxHadSelectionSet < firstHierarchyPanel.entries.Length - 1)
				DoArrowKeySelection (false);
			bool selectAll = leftCtrlKeyPressed && Keyboard.current.aKey.isPressed;
			if (selectAll && !prevSelectAll)
			{
				int prevSelectedCnt = 0;
				for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
				{
					HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
					prevSelectedCnt = hierarchyPanel.selected.Length;
					for (int i2 = 0; i2 < hierarchyPanel.entries.Length; i2 ++)
					{
						HierarchyEntry hierarchyEntry = hierarchyPanel.entries[i2];
						hierarchyEntry.SetSelected (true);
					}
				}
				InspectorPanel.RegenEntries (prevSelectedCnt > 1);
			}
			prevSelectAll = selectAll;
			bool doDuplicate = leftCtrlKeyPressed && Keyboard.current.dKey.isPressed;
			if (doDuplicate && !prevDoDuplicate)
				DuplicateSelected ();
			prevDoDuplicate = doDuplicate;
			if (Keyboard.current.f2Key.wasPressedThisFrame)
				RenameSelected ();
			if (Keyboard.current.deleteKey.wasPressedThisFrame)
				DeleteSelected ();
		}
		
		void DoArrowKeySelection (bool upArrowPressed)
		{
			int lastEntryIdxHadSelectionSet = HierarchyPanel.lastEntryIdxHadSelectionSet;
			for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
			{
				HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
				hierarchyPanel.entries[lastEntryIdxHadSelectionSet - upArrowPressed.PositiveOrNegative()].OnMouseDown ();
			}
		}

		public void NewPresetObject (int presetObjectTypeVal)
		{
			NewPresetObject ((PresetObjectType) Enum.ToObject(typeof(PresetObjectType), presetObjectTypeVal));
		}

		public _Object NewPresetObject (PresetObjectType presetObjectType)
		{
			if (presetObjectType == PresetObjectType.Empty)
				return NewObject();
			else
				throw new NotImplementedException();
		}

		public _Object NewObject (string name = "Object", Vector2 pos = new Vector2(), float rot = 0)
		{
			_Object ob = NewObject(obPrefab, name);

			return ob;
		}

		public _Object NewObject (_Object template, string name = "Object")
		{
			_Object ob = Instantiate(template);
			ob.name = GetUniqueName(name);
			ob.hierarchyEntries = new HierarchyEntry[HierarchyPanel.instances.Length];
			for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
			{
				HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
				HierarchyEntry hierarchyEntry = Instantiate(hierarchyEntryPrefab, hierarchyPanel.entriesParent);
				hierarchyEntry.nameText.text = ob.name;
				hierarchyEntry.ob = ob;
				hierarchyEntry.hierarchyPanel = hierarchyPanel;
				ob.hierarchyEntries[i] = hierarchyEntry;
				hierarchyPanel.entries = hierarchyPanel.entries.Add(hierarchyEntry);
			}
			ob.sceneEntries = new SceneEntry[0];
			for (int i = 0; i < template.components.Length; i ++)
			{
				_Component component = Instantiate(template.components[i]);
				component = Instantiate(component);
				component.ob = ob;
				SceneEntry sceneEntry = component.sceneEntry;
				if (sceneEntry != null)
				{
					sceneEntry = Instantiate(sceneEntry, sceneEntry.scenePanel.obsParentRectTrs);
					sceneEntry.hierarchyEntries = ob.hierarchyEntries;
					ob.sceneEntries = ob.sceneEntries.Add(sceneEntry);
					component.sceneEntry = sceneEntry;
				}
				for (int i2 = 0; i2 < component.inspectorEntries.Length; i2 ++)
				{
					InspectorEntry inspectorEntry = component.inspectorEntries[i2];
					inspectorEntry.gameObject.SetActive(false);
					inspectorEntry = Instantiate(inspectorEntry, inspectorEntry.rectTrs.parent);
					component.inspectorEntries[i2] = inspectorEntry;
					inspectorEntry.SetValueEntries (component);
					InspectorEntry[] inspectorEntriesForEntriesPrefabs = null;
					if (InspectorPanel.entreisForEntriesPrefabsDict.TryGetValue(component.inspectorEntryPrefab, out inspectorEntriesForEntriesPrefabs))
						InspectorPanel.entreisForEntriesPrefabsDict[component.inspectorEntryPrefab] = inspectorEntriesForEntriesPrefabs.Add(inspectorEntry);
					else
						InspectorPanel.entreisForEntriesPrefabsDict[component.inspectorEntryPrefab] = new InspectorEntry[] { inspectorEntry };
				}
				ob.components[i] = component;
				component.Init ();
			}
			obs = obs.Add(ob);
			return ob;
		}

		public _Component AddComponent (_Object ob, int componentPrefabIdx)
		{
			_Component componentPrefab = componentsPrefabs[componentPrefabIdx];
			for (int i = 0; i < ob.components.Length; i ++)
			{
				_Component _component = ob.components[i];
				if (_component.inspectorEntryPrefab.onlyAllowOnePerObject && componentPrefab.inspectorEntryPrefab == _component.inspectorEntryPrefab)
				{
					instance.onlyOneComponentPerObjectAllowedNotificationText.text = "There can't be multiple " + componentPrefab.name +  " components attached to the same object. " + ob.name +  " already contains a " + componentPrefab.name + " component.";
					instance.onlyOneComponentPerObjectAllowedNotificationGo.SetActive(true);
					return null;
				}
			}
			_Component component = Instantiate(componentPrefab);
			component.ob = ob;
			ob.components = ob.components.Add(component);
			component.Init ();
			InspectorPanel.AddOrUpdateEntries (component);
			return component;
		}

		public void AddCompnoentToSelected (int componentPrefabIdx)
		{
			HierarchyEntry[] selected = HierarchyPanel.instances[0].selected;
			for (int i = 0; i < selected.Length; i ++)
			{
				HierarchyEntry hierarchyEntry = selected[i];
				AddComponent (hierarchyEntry.ob, componentPrefabIdx);
			}
		}

		public void DuplicateSelected ()
		{
			HierarchyPanel firstHierarchyPanel = HierarchyPanel.instances[0];
			HierarchyEntry[] selected = firstHierarchyPanel.selected._Sort(new HierarchyEntry.Comparer());
			Dictionary<int, int> hierarchyEntriesIdxsDict = new Dictionary<int, int>();
			for (int i = 0; i < selected.Length; i ++)
			{
				HierarchyEntry hierarchyEntry = selected[i];
				string name = hierarchyEntry.ob.name;
				if (name.EndsWith(')'))
				{
					int spaceAndLeftParenthesisIdx = name.LastIndexOf(" (");
					int val;
					if (spaceAndLeftParenthesisIdx != -1 && int.TryParse(name.SubstringStartEnd(spaceAndLeftParenthesisIdx + 2, name.Length - 2), out val))
						name = name.Remove(spaceAndLeftParenthesisIdx);
				}
				NewObject (hierarchyEntry.ob, name);
				hierarchyEntriesIdxsDict[firstHierarchyPanel.entries.Length - 1] = hierarchyEntry.rectTrs.GetSiblingIndex() + 1 + hierarchyEntriesIdxsDict.Count;
			}
			for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
			{
				HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
				for (int i2 = 0; i2 < hierarchyPanel.selected.Length; i2 ++)
				{
					HierarchyEntry hierarchyEntry = hierarchyPanel.selected[i2];
					hierarchyEntry.SetSelected (false);
					i2 --;
				}
				foreach (KeyValuePair<int, int> keyValuePair in hierarchyEntriesIdxsDict)
				{
					HierarchyEntry hierarchyEntry = hierarchyPanel.entries[keyValuePair.Key];
					hierarchyEntry.Reorder (keyValuePair.Value);
					hierarchyEntry.SetSelected (true);
				}
			}
			InspectorPanel.RegenEntries (selected.Length > 1);
		}

		public void RenameSelected ()
		{
			for (int i = 0; i < HierarchyPanel.instances.Length; i ++)
			{
				HierarchyPanel hierarchyPanel = HierarchyPanel.instances[i];
				HierarchyEntry[] selected = hierarchyPanel.selected;
				if (selected.Length > 0)
				{
					if (selected.Length == 1)
					{
						HierarchyEntry hierarchyEntry = selected[0];
						hierarchyEntry.nameInputField.gameObject.SetActive(true);
						hierarchyEntry.nameInputField.text = hierarchyEntry.ob.name;
					}
					else
					{
						string nameOverlap = selected[0].ob.name.GetOverlapFromStart(selected[1].ob.name);
						for (int i2 = 2; i2 < selected.Length; i2 ++)
						{
							HierarchyEntry hierarchyEntry = selected[i2];
							nameOverlap = hierarchyEntry.ob.name.GetOverlapFromStart(nameOverlap);
						}
						for (int i2 = 0; i2 < selected.Length; i2 ++)
						{
							HierarchyEntry hierarchyEntry = selected[i2];
							hierarchyEntry.nameInputField.gameObject.SetActive(true);
							hierarchyEntry.nameInputField.text = nameOverlap;
						}
					}
				}
			}
		}

		public void DeleteSelected ()
		{
			
		}

		public void Export ()
		{
			string? pythonPath = GetPythonPath();
			if (pythonPath == null)
			{
				UnityEngine.Debug.LogError("Python not found. Please install Python to use the Export feature.");
				return;
			}
			for (int i = 0; i < obs.Length; i ++)
			{
				_Object ob = obs[i];
				if (ob.data.export.val)
					Export (ob);
			}
			string code = PYTHON;
			code = code.Replace("# Attributes", $"attributes = {attributes}");
			code = code.Replace("# API", apiCode);
			code = code.Replace("# Vars", string.Join('\n', vars));
			code = code.Replace("# UI Methods", string.Join('\n', uiMethods));
			Vector2 _gravity = new Vector2();
			if (useGravity.val)
				_gravity = gravity.val;
			List<string> physicsInitClauses = new List<string>(new string[] { "sim.set_length_unit (" + unitLen.val + ")\nsim.set_gravity (" + _gravity.x + ", " + _gravity.y + ")" });
			// for rigidBody in rigidBodies.values():
			// 	physicsInitClauses.append(rigidBody)
			// for collider in colliders.values():
			// 	physicsInitClauses.append(collider)
			// for joint in joints.values():
			// 	physicsInitClauses.append(joint)
			string physicsInitCode = "";
			for (int i = 0; i < physicsInitClauses.Count; i ++)
			{
				string clause = physicsInitClauses[i];
				string[] lines = clause.Split('\n');
				for (int i2 = 0; i2 < lines.Length; i2 ++)
				{
					string line = lines[i2];
					physicsInitCode += "	" + line + '\n';
				}
			}
			for (int i = 0; i < renderCode.Length; i ++)
			{
				string renderClause = renderCode[i];
				string _renderClause = "";
				string[] lines = renderClause.Split('\n');
				for (int i2 = 0; i2 < lines.Length; i2 ++)
				{
					string line = lines[i2];
					_renderClause += "	" + line + '\n';
				}
				renderCode[i] = _renderClause;
			}
			string particleSystemsCode = "";
			for (int i = 0; i < particleSystemsClauses.Length; i ++)
			{
				string particleSystemClause = particleSystemsClauses[i];
				string[] lines = particleSystemClause.Split('\n');
				for (int i2 = 0; i2 < lines.Length; i2 ++)
				{
					string line = lines[i2];
					particleSystemsCode += "	" + line + '\n';
				}
			}
			string uiCode = "";
			for (int i = 0; i < uiClauses.Length; i ++)
			{
				string uiClause = uiClauses[i];
				string[] lines = uiClause.Split('\n');
				for (int i2 = 0; i2 < lines.Length; i2 ++)
				{
					string line = lines[i2];
					uiCode += "	" + line + '\n';
				}
			}
			code = code.Replace("# Init Pivots, Attributes, UI", $"	pivots = {pivots}\n	attributes = {attributes}\n{uiCode}");
			code = code.Replace("# Init Physics", physicsInitCode);
			code = code.Replace("# Init Rendering", string.Join('\n', renderCode));
			code = code.Replace("# Init Particle Systems", particleSystemsCode);
			code = code.Replace("# Init User Code", string.Join('\n', initCode));
			for (int i = 0; i < updateScripts.Length; i ++)
			{
				string updateScript = updateScripts[i];
				string _updateScript = "";
				string[] lines = updateScript.Split('\n');
				for (int i2 = 0; i2 < lines.Length; i2 ++)
				{
					string line = lines[i2];
					_updateScript += "	" + line + '\n';
				}
				updateScripts[i] = updateScript;
			}
			code = code.Replace("# Globals", "	global " + string.Join(", ", globals));
			code = code.Replace("# Update", string.Join('\n', updateScripts));
			Color _backgroundColor = backgroundColor.val;
			code = code.Replace("# Background", "	screen.fill([" + _backgroundColor.r * 255 + ", " + _backgroundColor.g * 255 + ", " + _backgroundColor.b * 255 + "])");
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.FileName = pythonPath;
			processStartInfo.Arguments = "\"" + BUILD_SCRIPT_PATH;
			processStartInfo.UseShellExecute = false;
			processStartInfo.RedirectStandardError = true;
			processStartInfo.CreateNoWindow = false;
			processStartInfo.WorkingDirectory = Path.GetDirectoryName(BUILD_SCRIPT_PATH);
			try
			{
				Process process = Process.Start(processStartInfo);
				if (process == null)
					UnityEngine.Debug.LogError("Failed to start Python process. Check that Python is installed and accessible.");
				else
				{
					UnityEngine.Debug.Log($"Python process started with PID: {process.Id}");
					errorReader = process.StandardError;
					StartCoroutine(ReadErrors ());
				}
			}
			catch (Exception e)
			{
				UnityEngine.Debug.LogError($"Error launching Build script: {e.Message}\nStack trace: {e.StackTrace}");
			}
		}

		void Export (_Object ob)
		{
			string oVarName = GetVarNameForObject(ob);
			Dictionary<string, string> attributes = GetAttributes(ob);
			
		}

		string GetVarNameForObject (_Object ob)
		{
			string output = '_' + ob.name;
			string disallowedChars = " /\\`~?|!@#$%^&*()[]{}<>=+-;:',.\'";
			foreach (char disallowedChar in disallowedChars)
				output = output.Replace("" + disallowedChar, "");
			return output;
		}

		Dictionary<string, string> GetAttributes (_Object ob)
		{
			Dictionary<string, string> output = new Dictionary<string, string>();
			
			return output;
		}

		IEnumerator ReadErrors ()
		{
			while (true)
			{
				string line;
                while ((line = errorReader.ReadLine()) != null)
                    UnityEngine.Debug.LogError(line);
				yield return null;
			}
		}

		public static string GetUniqueName (string name, params _Object[] excludeObs)
		{
			string origName = name;
			int num = 1;
			while (true)
			{
				bool isValidName = true;
				for (int i = 0; i < obs.Length; i ++)
				{
					_Object ob = obs[i];
					if (ob.name == name && !excludeObs.Contains(ob))
					{
						isValidName = false;
						break;
					}
				}
				if (isValidName)
					return name;
				else
				{
					name = origName + " (" + num + ')';
					num ++;
				}
			}
		}

		public static string? GetPythonPath ()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return GetPythonPathOnWindows ();
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				return GetPythonPathOnUnix ();
			return null;
		}

		static string? GetPythonPathOnWindows ()
		{
			string? path = Environment.GetEnvironmentVariable("PATH");
			if (path != null)
			{
				string[] dirNames = path.Split(Path.PathSeparator);
				for (int i = 0; i < dirNames.Length; i ++)
				{
					string dirName = dirNames[i];
					string pythonPath = Path.Combine(dirName, "python.exe");
					if (File.Exists(pythonPath))
						return pythonPath;
					pythonPath = Path.Combine(dirName, "python3.exe");
					if (File.Exists(pythonPath))
						return pythonPath;
				}
			}
			return null;
		}

		static string? GetPythonPathOnUnix ()
		{
			string? path = Environment.GetEnvironmentVariable("PATH");
			if (path != null)
			{
				string[] dirNames = path.Split(Path.PathSeparator);
				for (int i = 0; i < dirNames.Length; i ++)
				{
					string dirName = dirNames[i];
					string pythonPath = Path.Combine(dirName, "python");
					if (File.Exists(pythonPath))
						return pythonPath;
					pythonPath = Path.Combine(dirName, "python3");
					if (File.Exists(pythonPath))
						return pythonPath;
				}
			}
			string[] commonPaths = { "/usr/bin/python", "/usr/local/bin/python", "/usr/bin/python3", "/usr/local/bin/python3" };
			foreach (string commonPath in commonPaths)
				if (File.Exists(commonPath))
					return commonPath;

			return null;
		}

		public enum PresetObjectType
		{
			Empty,
			Image,
			ParticleSystem
		}
	}
}