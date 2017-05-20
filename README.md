# RollABall-DI
Unity's Roll A Ball Tutorial modified to use Dependency Injection

## Introduction
I you have looked at the Unity Roll-a-ball tutorial https://unity3d.com/learn/tutorials/projects/roll-ball-tutorial you will have noticed a few things

1. Its really simple
2. if you are software engineer, you will have noticed its horrendously structured with a single god object (the ball) doing everything

My goal is to use Dependency Injection (DI) to rework the tutorial, and at the same time learn DI.

Allthough roll-a-ball is beginner level, the stuff I will be covering is intermediate advanced level. It aimed at experience programmers who understand how messed up large projects can get much too easily.

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
 * displaying of win message upon win
 
I hope you can recognise the anti-pattern "god object" here.  We certainly can improve on this.


