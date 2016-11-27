# FlipperControl
A control that uses flip transition to change different states for UWP apps.

Works on Build 10586 or higher because this control uses Composition API.

![Preview](https://github.com/JuniperPhoton/FlipperControl/blob/master/demo.gif)

#Get from Nuget

> Install-Package FlipperControl 

#How to use

    xmlns:flipper="using:FlipperControl"

    <flipper:FlipperControl Grid.Row="1"
                          AllowTapToFlip="True"
                          FlipDirection="BackToFront"
                          RotationAxis="X">
            <flipper:FlipperControl.Views>
               <!--Insert framework elements-->
            </flipper:FlipperControl.Views>
    </flipper:FlipperControl>
              
FlipperControlTest proj has demenstrated how to use this control.

There are a few properties that control the behavior:

## DisplayIndex property

The only way to change the visual state of the control. Note that the value of zero points to the last framework element you added in `FlipperControl.Views`. 

Note: Be aware of the **IndexOutOfRangeException **.

## AllowTapToFlip property

Tap to flip.
If there is a button on your view and you have set e.Handled=true, this will NOT work even if this property is enabled.

## RotationAxis property

Currently the value is either `X` or `Y`.

## FlipDirection property

Currently the value is either `FrontToBack` or `BackToFront`.

## AnimationDuration property

It's obvious.

## EnablePerspect property

The default value is true. 

When set this to true:


![Preview](https://github.com/JuniperPhoton/FlipperControl/blob/master/demo.gif)

When it's false:

![Preview](https://github.com/JuniperPhoton/FlipperControl/blob/master/no_p.gif)
