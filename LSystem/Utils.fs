namespace LSystem

open System
open System.Numerics

module Utils =
    
    let makeVector2 x y =
        Vector2(x,y)
    
    let rotate (v : Vector2) (theta : float32) =
        Vector2.Transform(v, Quaternion.CreateFromYawPitchRoll(0.f, 0.f, theta))

    let radians (x : float32) =
        x * (float32)Math.PI / 180.0f
    