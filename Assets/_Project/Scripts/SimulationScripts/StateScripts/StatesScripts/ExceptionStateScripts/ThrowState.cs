using UnityEngine;

public class ThrowState : State
{
    public override void UpdateLogic(PlayerNetwork player)
    {
        if (!player.enter)
        {
            if (player.direction.x < 0 && player.flip == 1 || player.direction.x > 0 && player.flip == -1)
            {
                player.flip *= -1;
            }
            player.enter = true;
            player.animationFrames = 0;
            player.animation = "Throw";
            player.attackFrames = DemonicsAnimator.GetMaxAnimationFrames(player.playerStats._animation, player.animation);
        }
        player.otherPlayer.pushbox.active = false;
        player.animationFrames++;
        player.attackFrames--;
        DemonicsVector2 grabPoint = player.player.PlayerAnimator.GetGrabPoint(player.animation, player.animationFrames);
        player.otherPlayer.position = new DemonicsVector2(player.position.x + (grabPoint.x * player.flip), player.position.y + grabPoint.y);
        if (player.player.PlayerAnimator.GetThrowArcanaEnd(player.animation, player.animationFrames) && player.otherPlayer.state == "Grabbed")
        {
        }
        ToIdleState(player);
    }
    private void ToIdleState(PlayerNetwork player)
    {
        if (player.attackFrames <= 0)
        {
            ThrowEnd(player.otherPlayer);
            if (player.attackNetwork.cameraShakerNetwork.timer > 0)
            {
                CameraShake.Instance.Shake(player.attackNetwork.cameraShakerNetwork);
            }
            player.enter = false;
            player.state = "Idle";
            player.sound = "Impact1";
        }
    }
}