<h1 align="center">Addler</h1>

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE.md)

[English Documents Available(英語ドキュメント)](README.md)

Unity の Addressable アセットシステムのためのメモリ管理システムです。  
ロードしたリソースのライフタイム管理、オブジェクトプーリング、プリローディング機能を提供します。

## 目次

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
<!-- param::title::詳細:: -->
<details>
<summary>詳細</summary>

- [概要とコンセプト](#%E6%A6%82%E8%A6%81%E3%81%A8%E3%82%B3%E3%83%B3%E3%82%BB%E3%83%97%E3%83%88)
- [セットアップ](#%E3%82%BB%E3%83%83%E3%83%88%E3%82%A2%E3%83%83%E3%83%97)
  - [要件](#%E8%A6%81%E4%BB%B6)
  - [インストール](#%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%88%E3%83%BC%E3%83%AB)
- [ライフタイムバインディング](#%E3%83%A9%E3%82%A4%E3%83%95%E3%82%BF%E3%82%A4%E3%83%A0%E3%83%90%E3%82%A4%E3%83%B3%E3%83%87%E3%82%A3%E3%83%B3%E3%82%B0)
  - [GameObjectにバインディングする](#gameobject%E3%81%AB%E3%83%90%E3%82%A4%E3%83%B3%E3%83%87%E3%82%A3%E3%83%B3%E3%82%B0%E3%81%99%E3%82%8B)
  - [GameObject以外にバインディングする](#gameobject%E4%BB%A5%E5%A4%96%E3%81%AB%E3%83%90%E3%82%A4%E3%83%B3%E3%83%87%E3%82%A3%E3%83%B3%E3%82%B0%E3%81%99%E3%82%8B)
- [プリローディング](#%E3%83%97%E3%83%AA%E3%83%AD%E3%83%BC%E3%83%87%E3%82%A3%E3%83%B3%E3%82%B0)
  - [プリローダの使い方](#%E3%83%97%E3%83%AA%E3%83%AD%E3%83%BC%E3%83%80%E3%81%AE%E4%BD%BF%E3%81%84%E6%96%B9)
  - [プリローダのライフタイムをバインディングする](#%E3%83%97%E3%83%AA%E3%83%AD%E3%83%BC%E3%83%80%E3%81%AE%E3%83%A9%E3%82%A4%E3%83%95%E3%82%BF%E3%82%A4%E3%83%A0%E3%82%92%E3%83%90%E3%82%A4%E3%83%B3%E3%83%87%E3%82%A3%E3%83%B3%E3%82%B0%E3%81%99%E3%82%8B)
  - [プリローディングの制約](#%E3%83%97%E3%83%AA%E3%83%AD%E3%83%BC%E3%83%87%E3%82%A3%E3%83%B3%E3%82%B0%E3%81%AE%E5%88%B6%E7%B4%84)
- [オブジェクトプーリング](#%E3%82%AA%E3%83%96%E3%82%B8%E3%82%A7%E3%82%AF%E3%83%88%E3%83%97%E3%83%BC%E3%83%AA%E3%83%B3%E3%82%B0)
  - [オブジェクトプーリングの使い方](#%E3%82%AA%E3%83%96%E3%82%B8%E3%82%A7%E3%82%AF%E3%83%88%E3%83%97%E3%83%BC%E3%83%AA%E3%83%B3%E3%82%B0%E3%81%AE%E4%BD%BF%E3%81%84%E6%96%B9)
  - [オブジェクトプールのライフタイムをバインディングする](#%E3%82%AA%E3%83%96%E3%82%B8%E3%82%A7%E3%82%AF%E3%83%88%E3%83%97%E3%83%BC%E3%83%AB%E3%81%AE%E3%83%A9%E3%82%A4%E3%83%95%E3%82%BF%E3%82%A4%E3%83%A0%E3%82%92%E3%83%90%E3%82%A4%E3%83%B3%E3%83%87%E3%82%A3%E3%83%B3%E3%82%B0%E3%81%99%E3%82%8B)
- [その他](#%E3%81%9D%E3%81%AE%E4%BB%96)
  - [プリローディングやオブジェクトプーリングを無効化する](#%E3%83%97%E3%83%AA%E3%83%AD%E3%83%BC%E3%83%87%E3%82%A3%E3%83%B3%E3%82%B0%E3%82%84%E3%82%AA%E3%83%96%E3%82%B8%E3%82%A7%E3%82%AF%E3%83%88%E3%83%97%E3%83%BC%E3%83%AA%E3%83%B3%E3%82%B0%E3%82%92%E7%84%A1%E5%8A%B9%E5%8C%96%E3%81%99%E3%82%8B)
  - [UniTaskを使う](#unitask%E3%82%92%E4%BD%BF%E3%81%86)
- [ライセンス](#%E3%83%A9%E3%82%A4%E3%82%BB%E3%83%B3%E3%82%B9)

</details>
<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## 概要とコンセプト
Addressable アセットシステムでは、ロードしたリソースが不要になったら明示的に解放をする必要があります。

```cs
// ロードする
var handle = Addressables.LoadAssetAsync<GameObject>("FooPrefab");
await handle.Task;

// ロードしたアセットを使用する
var prefab = handle.Result;

// 不要になったらリリースする
Addressables.Release(handle);
```

これを忘れるとメモリリークの原因となり、アプリケーションのクラッシュなど深刻な問題に繋がります。  
しかしながら、上記のような実装では、解放を忘れやすく、また忘れた時に気づきづらいという問題があります。

**Addler** ではこの問題に対応するため、以下のようにリソースのライフタイムを **GameObject** などに紐づけます。
紐づけた **GameObject** が破棄されると、リソースが自動的に解放されます。

```cs
var fooObj = new GameObject();

// fooObjにリソースのライフタイムを結びつける
// fooObjがDestroyされると同時にリソースもリリースされる
Addressables.LoadAssetAsync<GameObject>("BarPrefab").BindTo(fooObj);
```

上記のコードでは、`fooObj`が破棄されると同時にリソースが解放されます。
こうしておけば解放を忘れる心配はありません。

<p align="center">
  <img width=80% src="Documentation/concept_01.png" alt="Lifetime Binding">
</p>

またリソースを事前にロードしておき同期的にそれを取得するプリロード機能や、Prefabのインスタンスをプーリングして使いまわすオブジェクトプーリング機能も実装しています。

<p align="center">
  <img width=80% src="Documentation/concept_02.png" alt="Pooling">
</p>

さらにこれらのライフタイムも **GameObject** などにバインドし、解放漏れを防ぐことができます。

**Addler** はこのようにして **Addressable** におけるリソースのメモリを適切に管理するためのライブラリです。

## セットアップ

### 要件
- Unity 2020.3 以上

### インストール
1. Window > Package ManagerからPackage Managerを開く
2. 「+」ボタン > Add package from git URL
3. 以下を入力してインストール
   - https://github.com/Haruma-K/Addler.git?path=/Assets/Addler

<p align="center">
  <img width=50% src="Documentation/setup_01.png" alt="Package Manager">
</p>

あるいはPackages/manifest.jsonを開き、dependenciesブロックに以下を追記します。

```json
{
    "dependencies": {
        "com.harumak.addler": "https://github.com/Haruma-K/Addler.git?path=/Assets/Addler"
    }
}
```

バージョンを指定したい場合には以下のように末尾にバージョンを指定します。

- https://github.com/Haruma-K/Addler.git?path=/Assets/Addler#1.0.0

## ライフタイムバインディング

Addler の基本的な機能として、リソースのライフタイムを **GameObject** などと紐づけて、確実かつ自動的に解放処理を行うライフタイムバインディングがあります。

### GameObjectにバインディングする
**Addressable** で読み込んだリソースのライフタイムを **GameObject** と紐づけるには、以下のように`Addressables.LoadAsssetAsync()`の後ろに`BindTo()`と記述します。

```cs
// リソースをロードしてハンドルのライフタイムをgameObjectにバインドする
var handle = Addressables
    .LoadAssetAsync<GameObject>("FooPrefab")
    .BindTo(gameObject);
await handle.Task;
var prefab = handle.Result;

// gameObjectを破棄してハンドルをリリースする
Destroy(gameObject);
```

これで、gameObjectが破棄されると同時にリソースが解放されます。

### GameObject以外にバインディングする

ライフタイムは **GameObject** 以外にバインドすることもできます。**GameObject** 以外にバインドするためには `IReleaseEvent` を実装したクラスを作成し、`BindTo()`にそれを渡します。

**Addler** には **ParticleSystem** の終了タイミングにライフタイムをバインドするためのクラスを用意しています。`IReleaseEvent` の実装例として以下にこのクラスの実装を示します。

```cs
using System;
using Addler.Runtime.Core.LifetimeBinding;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public sealed class ParticleSystemBasedReleaseEvent : MonoBehaviour, IReleaseEvent // Implement IReleaseEvent
{
    [SerializeField] private ParticleSystem particle;
    private bool _isAliveAtLastFrame;

    private void Awake()
    {
        if (particle == null)
            particle = GetComponent<ParticleSystem>();
    }

    private void Reset()
    {
        particle = GetComponent<ParticleSystem>();
    }

    private void LateUpdate()
    {
        var isAlive = particle.IsAlive(true);
        if (_isAliveAtLastFrame && !isAlive)
            ReleasedInternal?.Invoke();

        _isAliveAtLastFrame = isAlive;
    }
    
    event Action IReleaseEvent.Dispatched
    {
        add => ReleasedInternal += value;
        remove => ReleasedInternal -= value;
    }

    private event Action ReleasedInternal;
}
```

## プリローディング
Addressables は基本的にリソースを非同期的にロードします。

```cs
//　Asynchronous loading
var handle = Addressables.LoadAssetAsync<GameObject>("fooPrefab");
await handle.Task;
```

しかし実際には、いわゆるロード画面で事前にリソースをロードして、ゲーム中は同期的にリソースをロードしたいというケースがあります。  
プリローダはこのような処理を実現するための機能です。

### プリローダの使い方
プリロードは `AddressablePreloader` クラスにより行います。
使い方は以下のコードの通りです。

```cs
using System;
using System.Collections;
using Addler.Runtime.Core.Preloading;
using UnityEngine;

public sealed class Example : MonoBehaviour
{
    private IEnumerator PreloadExample()
    {
        var preloader = new AddressablePreloader();

        // Preload
        {
            var progress = new Progress<float>(x => Debug.Log($"Progress: {x}"));
            
            // Preload by address.
            yield return preloader.PreloadKey<GameObject>("fooAddress", progress);

            // You can also preload by label.
            yield return preloader.PreloadKey<GameObject>("fooLabel", progress);

            // You can also preload multiple keys at once.
            yield return preloader.PreloadKeys<GameObject>(new[] { "barAddress", "bazAddress" }, progress);
        }

        // Get the preloaded object.
        {
            // Get by address.
            preloader.GetAsset<GameObject>("fooAddress");

            // Get multiple assets by label.
            preloader.GetAssets<GameObject>("fooLabel");
        }
        
        // Dispose the preloader and release all the assets.
        preloader.Dispose();
    }
}
```

`AddressablePreloader.PreloadKey/PreloadKeys`を呼ぶと引数に渡したキーが指すリソースを全てロードします。  
`AddressablesPreloader.GetAsset` メソッドを使うとプリロードしたリソースを同期的に取得できます。

プリローダを使用し終わったら`AddressablePool.Dispose`を呼ぶことですべてのリソースがリリースされます。

### プリローダのライフタイムをバインディングする
プリローダのライフタイムをバインドすることもできます。

```cs
// Bind the lifetime of the preloader to the GameObject.
// When gameObject is destroyed, the preloader will be disposed and release all the assets.
var preloader = new AddressablePreloader().BindTo(gameObject);
```

プリローダのライフタイムが終了するとすべてのリソースがリリースされます。

### プリローディングの制約

プリローディングの制約として、「プリロード時に指定したキーの種類」と「プリロードされたリソースを取得する際に指定したキーの種類」が一致している必要があります。  
例えば、アドレスAのアセットを含むラベルAをプリロード時に指定した場合、取得時にもラベルAを指定する必要があります。
アドレスAを指定して取得することはできません。

これは、**Addressable** アセットシステムの仕様上、アドレスやラベル、**AssetReference** が指すリソースのキー (PrimaryKey) を同期的に取得する手段がないためです。

もしこのようなケースに対応したい場合は **Addressables 1.17.1** からサポートされた [Synchronous Workflow](https://docs.unity3d.com/Packages/com.unity.addressables@1.21/manual/SynchronousAddressables.html) を使用することができます。
ただしこれには実行中の全ての **AsyncOperation** が終わるまで同期的に待つという仕様上の制約があるため、使用する際には注意が必要です。

## オブジェクトプーリング
Unity のゲームでは Prefab をインスタンス化した GameObject が多数使われます。  
しかし Prefab のインスタンス生成や破棄にはコストがかかり、頻繁に行いすぎるとパフォーマンスの低下を招きます。

例えば弾丸のように同じ Prefab のインスタンスを多数生成するようなケースでは、一定数のインスタンスをあらかじめ生成しておいてそれらを使いまわすことによりパフォーマンスの低下を防ぐことができます。  
これをオブジェクトプーリングと呼びます。

<p align="center">
  <img width=80% src="Documentation/concept_02.png" alt="Pooling">
</p>

Addler には Addressable アセットシステムでオブジェクトプーリングを扱うための機能が実装されています。

### オブジェクトプーリングの使い方
オブジェクトプールは`AddressablePool`クラスにより行います。  
使い方は以下のコードの通りです。

```cs
using System;
using System.Collections;
using Addler.Runtime.Core.Pooling;
using UnityEngine;

public sealed class Example : MonoBehaviour
{
    private IEnumerator PoolExample()
    {
        // Create a new pool.
        var pool = new AddressablePool("fooPrefab");

        // Create instances in the pool.
        var progress = new Progress<float>(x => Debug.Log($"Progress: {x}"));
        yield return pool.Warmup(5, progress);

        // Get an instance from the pool.
        var pooledObject = pool.Use();
        var instance = pooledObject.Instance;

        // Return the instance to the pool.
        pool.Return(pooledObject);
        //pooledObject.Dispose(); // You can also return the instance by disposing the pooled object.

        // Destroy the pool and release all instances.
        pool.Dispose();
    }
}
```

`AddressablePool.Warmup()`を呼ぶと引数に渡した数だけ Prefab のインスタンスが生成されます。  
プールからインスタンスを取得するには`AddressablePool.Use()`メソッドで`PooledObject`を取得します。  
これの`Instance`プロパティからインスタンスを取得できます。
`AddressablePool.Return` あるいは `PooledObject.Dispose()`メソッドを呼ぶとインスタンスがプールに戻ります。

プールを使用し終わったら`AddressablePool.Dispose()`でプールを破棄してください。
全てのインスタンスが破棄。解放されます。

### オブジェクトプールのライフタイムをバインディングする
オブジェクトプールや、プールから取得したオブジェクトのライフタイムをバインドすることもできます。

```cs
using System.Collections;
using Addler.Runtime.Core.Pooling;
using UnityEngine;

public sealed class Example : MonoBehaviour
{
    private IEnumerator PoolExample()
    {
        // Bind the lifetime of the pool to GameObject.
        // If gameObject1 is destroyed, the pool will be disposed.
        var pool = new AddressablePool("FooPrefab")
            .BindTo(gameObject1);

        yield return pool.Warmup(5);

        // Bind the lifetime of the instance to GameObject.
        // If gameObject2 is destroyed, the instance will be returned to the pool.
        var instance = pool
            .Use()
            .BindTo(gameObject2)
            .Instance;
    }
}
```

プールから取得したオブジェクトのライフタイムが終了するとプールに返却され、オブジェクトプールのライフタイムが終了するとすべてのインスタンスが破棄・リリースされます。

## その他

### プリローディングやオブジェクトプーリングを無効化する
**Addler** の機能のうち、プリローディングやオブジェクトプーリングを使用しない場合には、それらを無効化し、コンパイル対象から外すことができます。
無効化は **Player Settings** から以下の **Scripting Define Symbols** を設定することで行います。

- **ADDLER_DISABLE_PRELOADING** : プリローディングを無効化する
- **ADDLER_DISABLE_POOLING** : オブジェクトプーリングを無効化する

### UniTaskを使う
プリローディングやオブジェクトプーリングでは、コルーチンを使って非同期処理を待機します。
コルーチンの代わりにUniTaskを使いたい場合には以下の設定を行います。

1. [UniTask](https://github.com/Cysharp/UniTask) をインストールする（複数のインストール方法があります）
2. （Package Manager を経由しない方法で1.をインストールした場合のみ）**Scripting Define Symbols** に `ADDLER_UNITASK_SUPPORT` を追加して Unity を再起動する
3. `AddressablePool.WarmupAsync`などコルーチンを使っていたメソッドの **UniTask** 版が **Async** という接尾辞と共に使用可能になります

## ライセンス
本ソフトウェアはMITライセンスで公開しています。  
ライセンスの範囲内で自由に使っていただけますが、使用の際は以下の著作権表示とライセンス表示が必須となります。

* [LICENSE.md](LICENSE.md)

また、本ドキュメントの目次は以下のソフトウェアを使用して作成されています。

* [toc-generator](https://github.com/technote-space/toc-generator)

toc-generatorのライセンスの詳細は [Third Party Notices.md](Third%20Party%20Notices.md) を参照してください。
