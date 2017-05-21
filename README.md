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

The dependency injection container, that I'm using is Zenject https://github.com/modesttree/Zenject which is probably the best DI container for Unity.  The only trouble is that none of the people who write about DI seem to be able to explain it in simple terms or simple examples <sigh>. There is an article in the Unity blog on DI. I found it less than useful.

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
 
I hope you can recognise the anti-pattern "god object" here.  We certainly can improve on this.

## 1. Install Zenject and Wining the game
Head over to Unity Asset store & look for Zenject, it should be an easy find. If you look for 'dependency injection', you can find quite a bit of other tools. I chose Zenject as it reasonably easy and clear.

I also made a copy of the code (in the ZGame directory) so I can work on it without changing the original.  This is clearly a bad idea, its better to work on original code and let git keep the old versions, but I wanted to have the original always for comparison.

The simplest thing to seperate would be the "displaying of win message at right time" that has no dependencies and should be an easy fix.  To do this I will use Zenject's event system.  What way I can just fire off a 'you win' message and the GUI element can listen for that.  Its going to be 3 steps, setup messaging, fire the message and catch the message

### 1.1 Setup Messaging (and Zenject)


