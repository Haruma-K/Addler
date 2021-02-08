<h1 align="center">Addler</h1>
Preloading, Pooling, Lifetime Management System for Unity Addressable Asset System.

## Overview
In the Addressable asset system, loaded resources must be explicitly released when they are no longer needed.

```cs
// Load.
var handle = Addressables.LoadAssetAsync<GameObject>("FooPrefab");
await handle.Task;

// Release.
Addressables.Release(handle);
```

If you forget this release process, it will cause memory leaks and become a technical debt that will eventually lead to serious problems.

Addler simplifies memory management by binding resource lifetimes to GameObjects and other resources.

```cs
var fooObj = new GameObject();

// Load and bind the lifetime of the resource to fooObj.
// The asset will be released at the same time the fooObj is destroyed.
Addressables.LoadAssetAsync<GameObject>("BarPrefab").BindTo(fooObj);
```

In the above code, the resource is released as soon as the `fooObj` is destroyed.

If you bind the lifetime of a resource to a GameObject at load time like this, you don't have to worry about forgetting to release it.

<p align="center">
  <img width=500 src="https://user-images.githubusercontent.com/47441314/107219809-f0855880-6a54-11eb-9947-87e6948e4d60.png" alt="Lifetime Binding">
</p>

In addition, there is a preloader that preloads resources and get them synchronously, and also object pooling to reuse Prefab instances.

<p align="center">
  <img width=500 src="https://user-images.githubusercontent.com/47441314/107220479-b9fc0d80-6a55-11eb-81db-6cd7b855f4fb.png" alt="Object Pooling">
</p>

Naturally, these lifetimes can also be bound to GameObjects and other objects.

Addler is a library that uses these features to properly manage the lifetime of resources in Addressable Asset System.

## Install
Open Packages/manifest.json and add the following to the dependencies block.

```json
{
    "dependencies": {
        "com.harumak.addler": "https://github.com/Haruma-K/Addler.git?path=/Packages/com.harumak.addler"
    }
}
```

## Lifetime Binding
To bind the lifetime of a resource loaded by Addressable, write `BindTo()` after `Addressables.LoadAsssetAsync()`.

```cs
// Load the asset and bind the handle's lifetime to the GameObject.
var handle = Addressables.LoadAssetAsync<GameObject>("FooPrefab").BindTo(gameObject);
await handle.Task;
var prefab = handle.Result;

// Destroy the gameObject and release the bound handle.
Destroy(gameObject);
```

Now, the resource will be released as soon as the gameObject is destroyed.
Note that lifetimes can also be bound to non-GameObject (see below).

## Preloading
Addressables only provides an API to load resources asynchronously.

```cs
// Load asyncronously.
var handle = Addressables.LoadAssetAsync<GameObject>("FooPrefab");
await handle.Task;
```

However, in reality, in most cases you will want to load resources in advance at the "load screen" and load them synchronously during the game.
The preloader does this.

#### How to use Preloader
To use the preloader, instantiate the `AddressablesPreloader` class.

```cs
// Create the preloader.
var preloader = new AddressablesPreloader();

// Preload assets.
await preloader.PreloadAsync("FooPrefab", "BarPrefab");

// Get preloaded assets synchronously.
var fooPrefab = preloader.Get<GameObject>("FooPrefab");
var barPrefab = preloader.Get<GameObject>("BarPrefab");

// Release all.
preloader.Dispose();
```

You can preload a resource by passing its address to `AddressablesPreloader.PreloadAsync()`.
And you can get the preloaded resources synchronously by `AddressablesPreloader.Get()` method.

When you are done using the preloader, call `AddressablesPool.Dispose()` to release all the resources.

#### Preloader Lifetime Binding
You can also bind the lifetime of the preloader.

```cs
// Bind the lifetime of the preloader.
var preloader = new AddressablesPreloader().BindTo(gameObject);
```

All resources will be released when the preloader lifetime expires.

## Object Pooling
Unity games often use GameObjects instantiated from prefabs.
However, instantiating and destroying is expensive, and if done too often, it can take time to process.

Let's consider a case where many instances of the same Prefab are created, like bullets.
In such a case, we can reduce the processing load by using several instances created in advance.
This is called object pooling.

<p align="center">
  <img width=500 src="https://user-images.githubusercontent.com/47441314/107220479-b9fc0d80-6a55-11eb-81db-6cd7b855f4fb.png" alt="Object Pooling">
</p>

#### How to use Object Pooling
To use the object pool, instantiate the `AddressablesPool` class.

```cs
// Create the pool of the FooPrefab.
var pool = new AddressablesPool("FooPrefab");

// Create the instances.
await pool.WarmupAsync(5);

// Get the instance from the pool.
var operation= pool.Use();
var instance = operation.Object;

// Return the instance to the pool.
operation.Dispose();

// Destroy and release all the instances.
pool.Dispose();
```

When you call `AddressablesPool.WarmupAsync()`, as many instances of prefab as you pass in the argument will be created.
To get an instance from the pool, use the `AddressablesPool.Use()` method to get the `PooledObjectOperation`.
You can get an instance from the `Object` property of this.
To return the instance to the pool, call `PooledObjectOperation.Dispose()`.

When the pool is no longer needed, dispose it with `AddressablesPool.Dispose()`.
All instances will be destroyed and the resource will be released.

#### Object Pool Lifetime Binding
You can also bind the lifetime of the object pool and the lifetime of objects retrieved from the pool.

```cs
// Bind the lifetime of the pool.
var pool = new AddressablesPool("FooPrefab").BindTo(gameObject1);

await pool.WarmupAsync(5);

// Bind the lifetime of the instance.
// If the gameObject2 is destroyed, the instance will be returned to the pool.
var instance = pool.Use().BindTo(gameObject2).Object;
```

When the lifetime of an object retrieved from the pool expires, it will be returned to the pool.
When the lifetime of the object pool expires, all instances will be destroyed and released.

## Bind to Non-GameObject
You can also bind the lifetime to a non-GameObject.
To bind it to a non-GameObject, create a class that implements IEventDispatcher and pass it to BindTo().

Addler has a class for binding the lifetime to the end timing of ParticleSystem.
As an example implementation of IEventDispatcher, this class is shown below.

```cs
using System;
using UnityEngine;

namespace Addler.Runtime.Foundation.EventDispatcher
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSystemFinishedEventDispatcher : MonoBehaviour, IEventDispatcher // Implement IEventDispatcher
    {
        private bool _isAliveAtLastFrame;
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void LateUpdate()
        {
            var isAlive = _particleSystem.IsAlive(true);
            if (_isAliveAtLastFrame && !isAlive)
            {
                // Call OnDispatch when the ParticleSystem is finished.
                OnDispatch?.Invoke();
            }

            _isAliveAtLastFrame = isAlive;
        }

        public event Action OnDispatch;
    }
}
```
