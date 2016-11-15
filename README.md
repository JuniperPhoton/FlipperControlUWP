# FlipperControl
A control that uses flip transition to change different states.

![Preview](https://github.com/JuniperPhoton/FlipperControl/blob/master/demo.gif)

#USAGE
              <local:FlipperControl Grid.Row="1"
                                    AllowTapToFlip="True"
                                    FlipDirection="BackToFront"
                                    RotationAxis="X">
                  <local:FlipperControl.Views>
                      <!--Insert framework elements-->
                  </local:FlipperControl.Views>
              </local:FlipperControl>
              
FlipperControlTest proj has demenstrate how to use this control.

There are a few properties that control the behavior:

## DisplayIndex 

The only way to change the visual state of the control. Note that the value of zero points to the last framework element you added in `FlipperControl.Views`. Simply you can use this `DisplayIndex++` to loop the visual state and enjoy the flipping animation and no need to worry about the bound. 

##AllowTapToFlip

Tap to flip.
If there is a button on your view and you have set e.Handled=true, this will NOT work even if this property is enabled.

##RotationAxis

Currently the value is either X or Y.

##FlipDirection

Currently the value is either FrontToBack or BackToFront.
