# RollABall-DI
Unity's Roll A Ball Tutorial modified to use Dependency Injection

## Introduction
I you have looked at the Unity Roll-a-ball tutorial https://unity3d.com/learn/tutorials/projects/roll-ball-tutorial you will have noticed a few things

1. Its really simple
2. if you are software engineer, you will have noticed its horrendously structured with a single god object (the ball) doing everything

My goal is to use Dependency Injection (DI) to rework the tutorial, and at the same time learn DI.

Allthough roll-a-ball is beginner level, the stuff I will be covering is intermediate advanced level. It aimed at experience programmers who understand how messed up large projects can get much too easily.

## License
This code is MIT license (do with it what you feel like). The original Unity game was not particually licensed. Zenject itself is licensed under the MIT license, but I have not included it in this repo (why bother copying it?) so you will need to install that yourself.

## What is Dependency Injection
This 5 minute video gives a good summary, even though they talk php. https://www.youtube.com/watch?v=IKD2-MAkXyQ&t=212s I'm not going to repeat what it says, there is no point.  

The dependency injection container, that I'm using is Zenject https://github.com/modesttree/Zenject Which is a rather arbitary choice. It seems one of the better ones (I might rework this again using another DI system).  The only trouble is that none of the people who write about DI seem to be able to explain it in simple terms or simple examples <sigh>. There is an article in the Unity blog on DI, which I found less than useful.

Hopefully, what I write in this article might make sense.

## What is wrong with Roll-a-ball?
Start at the basics. Look at the Roll-a-ball code. We have 2 classes

* Rotator which rotates the object around
* PlayerController which does:
 * Movement of player
 * detection of player-pickup collision
 * give player score when pickup occurs
 * update of score GUI
 * removal of pickup
 * detection of win condition
 * displaying of win message at right time
 
I hope you can recognise the anti-pattern "god object" here.  We certainly can improve on this. Here is my more less coupled version:

* Rotator which rotates the object around
* PlayerController which moved the player about
* Pickup which:
 * detects of player-pickup collision
 * removal of pickup
* Score Manager which:
 * give player score when pickup occurs
 * detection of win condition
* Score Display which update of score GUI
* Win Display which displays the winmessage at right time

So the rest of the document will be on how I turned the original code into this.

## 1. Wining the game
The easiest piece of code to fix is the win-game. It has minimal coupling anyway. So we do this first.

Head over to Unity Asset store & look for Zenject, it should be an easy find.

I also made a copy of the code (in the ZGame directory) so I can work on it without changing the original.  This is clearly a bad idea, its better to work on original code and let git keep the old versions, but I wanted to have the original always for comparison.

Looking at the win message, it seems that using events will be better than a injected dependency, so we will use that

### 1.1 Setup Signals
Coming from a C/C++ background, I don't like having thousands of tiny files when there is no reason, therefore rather than one file per message, I created one file for all the messages I am using in this work:

``` csharp
// not a monobehaviour, just a holder for all the events
public static class ZSignals
{
    // the won game message
    public class GameWon : Signal<GameWon> { }
    // score has been updated message
    public class ScoreUpdated : Signal<int, ScoreUpdated> { }
}
```

Simple stuff: the `GameWon` signal is just a signal with no parameters. The `ScoreUpdated` will not be used until later but I gave it anyway.  All I need to do now is register this with the installer.

``` csharp
public class ZInstaller : MonoInstaller<ZInstaller>
{
    public override void InstallBindings()
    {
        Container.DeclareSignal<ZSignals.GameWon>();
    }
}
```
> In case you are wondering about the prefix:
>
> I prefixed all the code with a 'z' so I can tell it apart from the normal Unity example code.  If I ever test a different DI system, I will prefix that code with a different letter prefix. I don't recomend using the prefix, it just for clarity in the code.

### 1.2 Subscribe to the Signal
I'm adding a new behaviour to the 'you win' UI component.  Its purpose is to hide the UI until the signal arrives:
``` csharp
// this component will turn on/off the win message based upon an event
[RequireComponent(typeof(Text))]
public class ZWinMessage : MonoBehaviour
{
	// we want this signal (note the inject header)
    [Inject]
    ZSignals.GameWon onGameWonSignal; 

    private Text myText;
	// Use this for initialization
	void Start ()
    {
        myText = GetComponent<Text>();
        myText.enabled = false;

        // when we get the signal, call this fn
        // note variable is lowerCamel, but function it UpperCamel
        onGameWonSignal.Listen(OnGameWon);
    }

    private void OnDestroy()
    {
        onGameWonSignal.Unlisten(OnGameWon);
    }

    void OnGameWon()
    {
        Debug.Log("ZWinMessage gets GameWon");
        myText.enabled = true;
    }
}
```

> Tips:
> * I like to use `RequireComponent` to remind me to what other components a behaviour may need
> * Always have a `Debug.Log()` on your event handlers when first building, its a pain trying to debug if the message did/didn't get through correctly

### 1.3 Send the Signal
To do this we need we also need to remove the existing code from the player class. Our first step on tidying this code up:
``` csharp
public class ZPlayerController : MonoBehaviour
{
...
    // remove the winText
    [Inject]
    ZSignals.GameWon gameWonSignal;
...
    void SetCountText()
    {
    	// update score
        scoreText.text = "Score: "+count;
        // check for win game
        if (count >= 12)
        {
            Debug.Log("PlayerController says GameWon");
            gameWonSignal.Fire();
        }
    }
}
```
Again I have a comment before I send the message as well, that way I can check that the message was sent and it arrived as well.

This code has (currently) not given us much, but I for example we wanted a sound effect when the player wins and a transition to the next level.  I could have a sound manager which could subscribe to this signal and play at the right time.  I could also have a timer component which subscribes to the signal and transitions a few seconds after the signal is received.

If you test the code it will work fine, we are decoupling the code, but keeping it functional.

## 2. Updating the score
The next easiest piece of code to fix is the score UI.  While thinking about this I had two ideas:
* A signal `score-updated` with an integer parameter
* A `score-provider` component which you could get the score from
I decided in the end to the the signal as it felt neater. I didn't want the UI to be continually polling the score-provided, and if the score-provider also had an signal, then what does it give over the signal only solution.  Again there are 3 stages, the signal, the subscriber and the sender

### 2.1 The ScoreUpdated signal
We already mentioned the signal above, so there is just the zenject installer:
``` csharp
public class ZInstaller : MonoInstaller<ZInstaller>
{
    public override void InstallBindings()
    {
        Container.DeclareSignal<ZSignals.GameWon>();
        Container.DeclareSignal<ZSignals.ScoreUpdated>();
    }
}
```
Nothing else to see, moving on

### 2.2 The Score display
Again this is a new component which performs the task the player class used to:
``` csharp
[RequireComponent(typeof(Text))]
public class ZScoreDisplay : MonoBehaviour
{
    [Inject]
    ZSignals.ScoreUpdated onScoreUpdatedSignal; // we want this signal (note the inject header)

    private Text myText;
    // Use this for initialization
    void Start()
    {
        myText = GetComponent<Text>();
        // call the update fn to set the inital score to zero
        // if we had a ref to the ScoreManager we could instead have got it from that
        // or made the score manager emit an event at startup
        OnScoreUpdate(0);

        // when we get the signal, call this fn
        onScoreUpdatedSignal.Listen(OnScoreUpdate);
    }

    private void OnDestroy()
    {
        onScoreUpdatedSignal.Unlisten(OnScoreUpdate);
    }

    void OnScoreUpdate(int value)
    {
        Debug.LogFormat("ZScoreDisplay.OnScoreUpdate {0}",value);
        myText.text = "Score: "+value;
    }
}
```

> To make sure the score was set to zero at the start, I added an extra call just to get it setup properly


### 2.3 Sending the score out
For now (only) this code is inside the player class. I could have moved all the code at the same time, but I wanted to show how we can change this bit by bit:
``` csharp
public class ZPlayerController : MonoBehaviour
{
...
    // remove the scoreText
    [Inject]
    ZSignals.ScoreUpdated scoreUpdatedSignal;
...
    void SetCountText()
    {
    	// update score
        scoreUpdatedSignal.Fire(count);
        ... // check for win game
    }
}
``` 
Our next stage is going to be the big one.


## 3. The ScoreManager
If you look back at the original design idea, there was a score manager which keeps track to the score and then updates the UI.  Thinking about this again I was thinking about the signal/injection choice.

* If I have a third signal `item-picked-up` the collision could fire this off, and it would then fire off 1 or 2 other signals. Which is starting to get quite a lot
* If we have an interface which is just called it seems neater

> In some event based systems I have seen event cascades where event-A fires off event-B which fires off event-C.  Suddenly you have 20 events at once an no idea who fired the first one off.
>
> Even worse is if event-A triggers event-B which triggers event-A... you then have an unlimited supply of events followed by a crash.

### 3.1 Writing the ScoreManager

The code ended up like this:
``` csharp
public class ZScoreManager
{
    private int count=0; // the 'score'

    [Inject]
    ZSignals.GameWon gameWonSignal;
    [Inject]
    ZSignals.ScoreUpdated scoreUpdatedSignal;

    // this is only part of the class which is meant to be called
    // if this were an interface this is the only function we would have
    public void AddScore(int amount)
    {
        count += amount;
        scoreUpdatedSignal.Fire(count);
        if (count>=12) // don't like this have coded number, but will leave for now
        {
            gameWonSignal.Fire();
        }
    }
}
``` 

Notes:
* Its NOT a monobehaviour.  When I started writing, I assumed it would be, but when I finished the code, I realised that it doesn't need a GameObject, so it ended up pure C#
* Its really simple (which is a good this)
* There is no interface.
 * I could have written one, it would only need the `AddScore()` function, but I don't like writing tiny classes for not reason, so I didn't bother.  
 * Purists will probably condem me for violating the [dependency interface principle](https://en.wikipedia.org/wiki/Dependency_inversion_principle) 
 * I don't see the value of having a extra file and interface when there is only one user of the class and one implementation, its just overhead and more code to maintain
* I didn't like the hard coded 12 in the code, but I left it. If I was to improve, I might have some kind of pickup manager which would either count the total object or similar.

We also need to update the installer:
``` csharp
public class ZInstaller : MonoInstaller<ZInstaller>
{
    public override void InstallBindings()
    {
        Container.DeclareSignal<ZSignals.GameWon>();
        Container.DeclareSignal<ZSignals.ScoreUpdated>();
        Container.Bind<ZScoreManager>().AsSingle();
    }
}
```

### 3.2 Updating the player

As I was about to add this into the player I realised:
* No change is needed to either the `ZWinMessage` or the `ZScoreDisplay` classes
 * They just get a signal from somewhere and act upon it
 * It may be a different sender, but it makes no difference to them
 * Win!
* The `ZPlayerController` is now losing a lot of code and becoming simpler too
 * Also Win!

The code player controller is now:

``` csharp
public class ZPlayerController : MonoBehaviour
{
...
	// remove the UI components and signals, we only need
    // remove the winText
    [Inject]
    ZScoreManager scoreManager;
...
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            other.gameObject.SetActive(false);
            scoreManager.AddScore(1);   // tell the score manager, it does the rest
        }
    }
    // all score code is now gone..
}

```

Testing the game it works just like normal, the magic is that all the code is moving away from the player class, but its still works fine.

## 4. Final: Moving the collision detection code
To be honest, this code is looking fine now, but just finishing it off.

Consider if we moves to 5 different types of pickup:
* If the collision detection code stays in the player, we end up with a long if/then for the various types
* If the collision is in the pickup, then its one class per pickup

So lets move the code.  We need a pickup class and an updated player class:
``` csharp
public class ZPickup : MonoBehaviour
{
    [Inject]
    ZScoreManager scoreManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);    // turn me off
            scoreManager.AddScore(1);   // tell the score manager, it does the rest
        }
    }
}
```

``` csharp
public class ZPlayerController : MonoBehaviour
{
    public float speed;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * speed);
    }
}
```

Now the player controller only does one thing. Move the player about.

# Conclusion
This document was mainly for me, to learn and teach myself how to use DI. If you find it useful, great. It doesn't cover the advanced stuff like injecting with prefabs or object pools. But Unity's roll-a-ball wasn't really complex enough to need that stuff, and I didn't want to make a simple roll-a-ball into some complex monster (looks towards the Zenject examples and frowns).  Hope this is useful.