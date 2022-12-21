using SharedGame;
using System;
using System.IO;
using Unity.Collections;
using UnityEngine;

[Serializable]
public struct EffectGroupNetwork
{
    public Vector2 position;
    public int animationFrames;
    public bool active;
    public bool flip;

    public void Serialize(BinaryWriter bw)
    {
        bw.Write(position.x);
        bw.Write(position.y);
        bw.Write(animationFrames);
        bw.Write(active);
        bw.Write(flip);
    }

    public void Deserialize(BinaryReader br)
    {
        position.x = br.ReadSingle();
        position.y = br.ReadSingle();
        animationFrames = br.ReadInt32();
        active = br.ReadBoolean();
        flip = br.ReadBoolean();
    }
};
[Serializable]
public struct EffectNetwork
{
    public string name;
    public int animationMaxFrames;
    public EffectGroupNetwork[] effectGroups;

    public void Serialize(BinaryWriter bw)
    {
        bw.Write(name);
        bw.Write(animationMaxFrames);
        for (int i = 0; i < effectGroups.Length; ++i)
        {
            effectGroups[i].Serialize(bw);
        }
    }

    public void Deserialize(BinaryReader br)
    {
        name = br.ReadString();
        animationMaxFrames = br.ReadInt32();
        for (int i = 0; i < effectGroups.Length; ++i)
        {
            effectGroups[i].Deserialize(br);
        }
    }
};
[Serializable]
public class PlayerNetwork
{
    public PlayerStatsSO playerStats;
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 direction;
    public string animation;
    public int animationFrames;
    public int flip;
    public string sound;
    public string soundStop;
    public float gravity;
    public float jump;
    public int dashDirection;
    public int dashFrames;
    public bool canDash;
    public bool hasJumped;
    public bool canJump;
    public bool canDoubleJump;
    public bool start;
    public bool skip;
    public string state;
    public States CurrentState;
    public EffectNetwork[] effects;
    public void Serialize(BinaryWriter bw)
    {
        bw.Write(position.x);
        bw.Write(position.y);
        bw.Write(velocity.x);
        bw.Write(velocity.y);
        bw.Write(direction.x);
        bw.Write(direction.y);
        bw.Write(animation);
        bw.Write(animationFrames);
        bw.Write(sound);
        bw.Write(soundStop);
        bw.Write(gravity);
        bw.Write(canDash);
        bw.Write(jump);
        bw.Write(hasJumped);
        bw.Write(canJump);
        bw.Write(canDoubleJump);
        bw.Write(dashDirection);
        bw.Write(dashFrames);
        bw.Write(start);
        bw.Write(skip);
        bw.Write(flip);
        bw.Write(state);
        for (int i = 0; i < effects.Length; ++i)
        {
            effects[i].Serialize(bw);
        }
    }

    public void Deserialize(BinaryReader br)
    {
        position.x = br.ReadSingle();
        position.y = br.ReadSingle();
        velocity.x = br.ReadSingle();
        velocity.y = br.ReadSingle();
        direction.x = br.ReadSingle();
        direction.y = br.ReadSingle();
        animation = br.ReadString();
        animationFrames = br.ReadInt32();
        sound = br.ReadString();
        soundStop = br.ReadString();
        gravity = br.ReadSingle();
        canDash = br.ReadBoolean();
        jump = br.ReadSingle();
        hasJumped = br.ReadBoolean();
        canJump = br.ReadBoolean();
        canDoubleJump = br.ReadBoolean();
        dashDirection = br.ReadInt32();
        dashFrames = br.ReadInt32();
        start = br.ReadBoolean();
        skip = br.ReadBoolean();
        flip = br.ReadInt32();
        state = br.ReadString();
        for (int i = 0; i < effects.Length; ++i)
        {
            effects[i].Deserialize(br);
        }
    }

    public override int GetHashCode()
    {
        int hashCode = 1858597544;
        hashCode = hashCode * -1521134295 + position.GetHashCode();
        hashCode = hashCode * -1521134295 + velocity.GetHashCode();
        hashCode = hashCode * -1521134295 + direction.GetHashCode();
        hashCode = hashCode * -1521134295 + start.GetHashCode();
        hashCode = hashCode * -1521134295 + animation.GetHashCode();
        hashCode = hashCode * -1521134295 + hasJumped.GetHashCode();
        hashCode = hashCode * -1521134295 + canJump.GetHashCode();
        hashCode = hashCode * -1521134295 + canDoubleJump.GetHashCode();
        hashCode = hashCode * -1521134295 + dashFrames.GetHashCode();
        hashCode = hashCode * -1521134295 + flip.GetHashCode();
        hashCode = hashCode * -1521134295 + sound.GetHashCode();
        hashCode = hashCode * -1521134295 + skip.GetHashCode();
        return hashCode;
    }

    public void SetEffect(string name, Vector2 position, bool flip = false)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            if (name == effects[i].name)
            {
                for (int j = 0; j < effects[i].effectGroups.Length; j++)
                {
                    if (!effects[i].effectGroups[j].active)
                    {
                        effects[i].effectGroups[j].flip = flip;
                        effects[i].effectGroups[j].active = true;
                        effects[i].effectGroups[j].position = position;
                        break;
                    }
                }
            }
        }
    }
};

[Serializable]
public struct VwGame : IGame
{
    public int Framenumber { get; private set; }
    public int NextFramenumber { get; private set; }
    public int Checksum => GetHashCode();

    public static PlayerNetwork[] _players;

    public void Serialize(BinaryWriter bw)
    {
        bw.Write(Framenumber);
        bw.Write(NextFramenumber);
        bw.Write(_players.Length);
        for (int i = 0; i < _players.Length; ++i)
        {
            _players[i].Serialize(bw);
        }
    }

    public void Deserialize(BinaryReader br)
    {
        Framenumber = br.ReadInt32();
        NextFramenumber = br.ReadInt32();
        int length = br.ReadInt32();
        if (length != _players.Length)
        {
            _players = new PlayerNetwork[length];
        }
        for (int i = 0; i < _players.Length; ++i)
        {
            _players[i].Deserialize(br);
        }
    }

    public NativeArray<byte> ToBytes()
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                Serialize(writer);
            }
            return new NativeArray<byte>(memoryStream.ToArray(), Allocator.Persistent);
        }
    }

    public void FromBytes(NativeArray<byte> bytes)
    {
        using (var memoryStream = new MemoryStream(bytes.ToArray()))
        {
            using (var reader = new BinaryReader(memoryStream))
            {
                Deserialize(reader);
            }
        }
    }

    /*
     * InitGameState --
     *
     * Initialize our game state.
     */

    public VwGame(PlayerStatsSO[] playerStats)
    {
        Framenumber = 0;
        NextFramenumber = 1;
        _players = new PlayerNetwork[playerStats.Length];
        ObjectPoolingManager.Instance.PoolInitialize(playerStats[0]._effectsLibrary, playerStats[1]._effectsLibrary);
        for (int i = 0; i < _players.Length; i++)
        {
            _players[i] = new PlayerNetwork();
            _players[i].state = "Idle";
            _players[i].position = new Vector2(GameplayManager.Instance.GetSpawnPositions()[i], (float)DemonicsPhysics.GROUND_POINT);
            _players[i].playerStats = playerStats[i];
            _players[i].animation = "Idle";
            _players[i].sound = "";
            _players[i].soundStop = "";
            _players[i].gravity = 0.018f;
            _players[i].canJump = true;
            _players[i].canDoubleJump = true;
            _players[i].effects = new EffectNetwork[playerStats[i]._effectsLibrary._objectPools.Count];
            for (int j = 0; j < _players[i].effects.Length; j++)
            {
                _players[i].effects[j] = new EffectNetwork();
                _players[i].effects[j].name = playerStats[i]._effectsLibrary._objectPools[j].groupName;
                _players[i].effects[j].animationMaxFrames = ObjectPoolingManager.Instance.GetObjectAnimation(i, _players[i].effects[j].name).GetMaxAnimationFrames();
                _players[i].effects[j].effectGroups = new EffectGroupNetwork[playerStats[i]._effectsLibrary._objectPools[j].size];
            }
        }
    }

    public void ParseInputs(long inputs, int i, out bool skip, out bool up, out bool down, out bool left, out bool right, out bool light, out bool medium,
    out bool heavy, out bool arcana, out bool grab, out bool shadow, out bool blueFrenzy, out bool redFrenzy, out bool dashForward, out bool dashBackward)
    {
        if ((inputs & NetworkInput.SKIP_BYTE) != 0)
        {
            skip = true;
        }
        else
        {
            skip = false;
        }
        if ((inputs & NetworkInput.UP_BYTE) != 0)
        {
            up = true;
        }
        else
        {
            up = false;
        }
        if ((inputs & NetworkInput.DOWN_BYTE) != 0)
        {
            down = true;
        }
        else
        {
            down = false;
        }
        if ((inputs & NetworkInput.LEFT_BYTE) != 0)
        {
            left = true;
        }
        else
        {
            left = false;
        }
        if ((inputs & NetworkInput.RIGHT_BYTE) != 0)
        {
            right = true;
        }
        else
        {
            right = false;
        }
        if ((inputs & NetworkInput.LIGHT_BYTE) != 0)
        {
            light = true;
        }
        else
        {
            light = false;
        }
        if ((inputs & NetworkInput.MEDIUM_BYTE) != 0)
        {
            medium = true;
        }
        else
        {
            medium = false;
        }
        if ((inputs & NetworkInput.HEAVY_BYTE) != 0)
        {
            heavy = true;
        }
        else
        {
            heavy = false;
        }
        if ((inputs & NetworkInput.ARCANA_BYTE) != 0)
        {
            arcana = true;
        }
        else
        {
            arcana = false;
        }
        if ((inputs & NetworkInput.GRAB_BYTE) != 0)
        {
            grab = true;
        }
        else
        {
            grab = false;
        }
        if ((inputs & NetworkInput.SHADOW_BYTE) != 0)
        {
            shadow = true;
        }
        else
        {
            shadow = false;
        }
        if ((inputs & NetworkInput.BLUE_FRENZY_BYTE) != 0)
        {
            blueFrenzy = true;
        }
        else
        {
            blueFrenzy = false;
        }
        if ((inputs & NetworkInput.RED_FRENZY_BYTE) != 0)
        {
            redFrenzy = true;
        }
        else
        {
            redFrenzy = false;
        }
        if ((inputs & NetworkInput.DASH_FORWARD_BYTE) != 0)
        {
            dashForward = true;
        }
        else
        {
            dashForward = false;
        }
        if ((inputs & NetworkInput.DASH_BACKWARD_BYTE) != 0)
        {
            dashBackward = true;
        }
        else
        {
            dashBackward = false;
        }
    }

    public void PlayerLogic(int index, bool skip, bool up, bool down, bool left, bool right, bool light, bool medium, bool heavy,
    bool arcana, bool grab, bool shadow, bool blueFrenzy, bool redFrenzy, bool dashForward, bool dashBackward)
    {
        if (up)
        {
            _players[index].direction = new Vector2(0, 1);
        }
        if (down)
        {
            _players[index].direction = new Vector2(0, -1);
        }
        if (right)
        {
            _players[index].direction = new Vector2(1, _players[index].direction.y);
        }
        if (left)
        {
            _players[index].direction = new Vector2(-1, _players[index].direction.y);
        }
        if (dashForward)
        {
            _players[index].dashDirection = 1;
            _players[index].CurrentState.ToDashState(_players[index]);
        }
        if (dashBackward)
        {
            _players[index].dashDirection = -1;
            _players[index].CurrentState.ToDashState(_players[index]);
        }
        if (!left && !right)
        {
            _players[index].direction = new Vector2(0, _players[index].direction.y);
        }
        if (!up && !down)
        {
            _players[index].direction = new Vector2(_players[index].direction.x, 0);
        }
        if (!left && !right)
        {
            _players[index].direction = new Vector2(0, _players[index].direction.y);
        }
        if (!up && !down)
        {
            _players[index].direction = new Vector2(_players[index].direction.x, 0);
        }
        // _players[index].CurrentState.UpdateLogic(_players[index]);
        // if (_players[index].CurrentState.NextState != null && _players[index].CurrentState.NextState != _players[index].CurrentState)
        // {
        //     _players[index].CurrentState = _players[index].CurrentState.NextState;
        //     _players[index].CurrentState.Enter(_players[index]);
        // }
        // if (_players[index].dashFrames > 0)
        // {
        //     _players[index].velocity = new Vector2(_players[index].dashDirection * (float)_players[index].playerStats.DashForce, 0);
        //     _players[index].position = new Vector2(_players[index].position.x + _players[index].velocity.x, (float)DemonicsPhysics.GROUND_POINT);
        //     _players[index].dashFrames--;
        // }
        // else
        // {
        //     _players[index].velocity = new Vector2(0, 0);
        //     _players[index].position = new Vector2(_players[index].position.x + _players[index].velocity.x, (float)DemonicsPhysics.GROUND_POINT);
        // }
        if (_players[index].state == "Idle")
        {
            _players[index].CurrentState = new IdleStates();
        }
        if (_players[index].state == "Walk")
        {
            _players[index].CurrentState = new WalkStates();
        }
        if (_players[index].state == "Dash")
        {
            _players[index].CurrentState = new DashStates();
        }
        if (_players[index].state == "DashAir")
        {
            _players[index].CurrentState = new DashAirState();
        }
        if (_players[index].state == "Run")
        {
            _players[index].CurrentState = new RunStates();
        }
        if (_players[index].state == "JumpForward")
        {
            _players[index].CurrentState = new JumpForwardStates();
        }
        if (_players[index].state == "Jump")
        {
            _players[index].CurrentState = new JumpStates();
        }
        if (_players[index].state == "Fall")
        {
            _players[index].CurrentState = new FallStates();
        }
        if (_players[index].state == "Crouch")
        {
            _players[index].CurrentState = new CrouchStates();
        }


        _players[index].CurrentState.UpdateLogic(_players[index]);
        if (index == 0)
        {
            if (!Collision(_players[0], _players[1]))
            {
                _players[index].position = new Vector2(_players[index].position.x, _players[index].position.y);
            }
        }
        else
        {
            if (!Collision(_players[1], _players[0]))
            {
                _players[index].position = new Vector2(_players[index].position.x, _players[index].position.y);
            }
        }
        if (GameplayManager.Instance.PlayerOne)
        {
            if ((DemonicsFloat)_players[index].position.x >= DemonicsPhysics.WALL_RIGHT_POINT && (DemonicsFloat)_players[index].velocity.x >= (DemonicsFloat)0)
            {
                _players[index].position = new Vector2((float)DemonicsPhysics.WALL_RIGHT_POINT, _players[index].position.y);
            }
            if ((DemonicsFloat)_players[index].position.x <= DemonicsPhysics.WALL_LEFT_POINT && (DemonicsFloat)_players[index].velocity.x <= (DemonicsFloat)0)
            {
                _players[index].position = new Vector2((float)DemonicsPhysics.WALL_LEFT_POINT, _players[index].position.y);
            }
        }
        if (GameplayManager.Instance.PlayerOne)
        {
            _players[0].flip = GameplayManager.Instance.PlayerOne.IsFlip();
            _players[1].flip = GameplayManager.Instance.PlayerTwo.IsFlip();
            if (GameplayManager.Instance.PlayerOne.IsAnimationFinished())
            {
                _players[0].animationFrames = 0;
            }
        }
        if (GameplayManager.Instance.PlayerTwo)
        {
            if (GameplayManager.Instance.PlayerTwo.IsAnimationFinished())
            {
                _players[1].animationFrames = 0;
            }
        }
        for (int i = 0; i < _players[index].effects.Length; i++)
        {
            for (int j = 0; j < _players[index].effects[i].effectGroups.Length; j++)
            {
                if (_players[index].effects[i].effectGroups[j].active)
                {
                    _players[index].effects[i].effectGroups[j].animationFrames++;
                    if (_players[index].effects[i].effectGroups[j].animationFrames >= _players[index].effects[i].animationMaxFrames)
                    {
                        _players[index].effects[i].effectGroups[j].animationFrames = 0;
                        _players[index].effects[i].effectGroups[j].active = false;
                    }
                }
            }
        }
    }

    public void Update(long[] inputs, int disconnect_flags)
    {
        if (Time.timeScale > 0)
        {
            Framenumber++;
            DemonicsWorld.Frame = Framenumber;
            for (int i = 0; i < _players.Length; i++)
            {
                bool skip = false;
                bool up = false;
                bool down = false;
                bool left = false;
                bool right = false;
                bool light = false;
                bool medium = false;
                bool heavy = false;
                bool arcana = false;
                bool grab = false;
                bool shadow = false;
                bool blueFrenzy = false;
                bool redFrenzy = false;
                bool dashForward = false;
                bool dashBackward = false;
                if ((disconnect_flags & (1 << i)) != 0)
                {
                    //AI
                }
                else
                {
                    ParseInputs(inputs[i], i, out skip, out up, out down, out left, out right, out light, out medium, out heavy, out arcana,
                     out grab, out shadow, out blueFrenzy, out redFrenzy, out dashForward, out dashBackward);
                }
                PlayerLogic(i, skip, up, down, left, right, light, medium, heavy, arcana, grab, shadow, blueFrenzy, redFrenzy, dashForward, dashBackward);
                _players[i].start = true;
            }
        }
    }

    public long ReadInputs(int id)
    {
        long input = 0;
        if (id == 0)
        {
            if (Input.anyKeyDown)
            {
                input |= NetworkInput.SKIP_BYTE;
            }
            if (NetworkInput.ONE_UP_INPUT)
            {
                input |= NetworkInput.UP_BYTE;
            }
            if (NetworkInput.ONE_DOWN_INPUT)
            {
                input |= NetworkInput.DOWN_BYTE;
            }
            if (NetworkInput.ONE_LEFT_INPUT)
            {
                input |= NetworkInput.LEFT_BYTE;
            }
            if (NetworkInput.ONE_RIGHT_INPUT)
            {
                input |= NetworkInput.RIGHT_BYTE;
            }
            if (NetworkInput.ONE_LIGHT_INPUT)
            {
                input |= NetworkInput.LIGHT_BYTE;
                NetworkInput.ONE_LIGHT_INPUT = false;
            }
            if (NetworkInput.ONE_MEDIUM_INPUT)
            {
                input |= NetworkInput.MEDIUM_BYTE;
                NetworkInput.ONE_MEDIUM_INPUT = false;
            }
            if (NetworkInput.ONE_HEAVY_INPUT)
            {
                input |= NetworkInput.HEAVY_BYTE;
                NetworkInput.ONE_HEAVY_INPUT = false;
            }
            if (NetworkInput.ONE_ARCANA_INPUT)
            {
                input |= NetworkInput.ARCANA_BYTE;
                NetworkInput.ONE_ARCANA_INPUT = false;
            }
            if (NetworkInput.ONE_SHADOW_INPUT)
            {
                input |= NetworkInput.SHADOW_BYTE;
                NetworkInput.ONE_SHADOW_INPUT = false;
            }
            if (NetworkInput.ONE_GRAB_INPUT)
            {
                input |= NetworkInput.GRAB_BYTE;
                NetworkInput.ONE_GRAB_INPUT = false;
            }
            if (NetworkInput.ONE_BLUE_FRENZY_INPUT)
            {
                input |= NetworkInput.BLUE_FRENZY_BYTE;
                NetworkInput.ONE_BLUE_FRENZY_INPUT = false;
            }
            if (NetworkInput.ONE_RED_FRENZY_INPUT)
            {
                input |= NetworkInput.RED_FRENZY_BYTE;
                NetworkInput.ONE_RED_FRENZY_INPUT = false;
            }
            if (NetworkInput.ONE_DASH_FORWARD_INPUT)
            {
                input |= NetworkInput.DASH_FORWARD_BYTE;
                NetworkInput.ONE_DASH_FORWARD_INPUT = false;
            }
            if (NetworkInput.ONE_DASH_BACKWARD_INPUT)
            {
                input |= NetworkInput.DASH_BACKWARD_BYTE;
                NetworkInput.ONE_DASH_BACKWARD_INPUT = false;
            }
        }
        if (id == 1)
        {
            if (Input.anyKeyDown)
            {
                input |= NetworkInput.SKIP_BYTE;
            }
            if (NetworkInput.TWO_UP_INPUT)
            {
                input |= NetworkInput.UP_BYTE;
            }
            if (NetworkInput.TWO_DOWN_INPUT)
            {
                input |= NetworkInput.DOWN_BYTE;
            }
            if (NetworkInput.TWO_LEFT_INPUT)
            {
                input |= NetworkInput.LEFT_BYTE;
            }
            if (NetworkInput.TWO_RIGHT_INPUT)
            {
                input |= NetworkInput.RIGHT_BYTE;
            }
            if (NetworkInput.TWO_LIGHT_INPUT)
            {
                input |= NetworkInput.LIGHT_BYTE;
                NetworkInput.TWO_LIGHT_INPUT = false;
            }
            if (NetworkInput.TWO_MEDIUM_INPUT)
            {
                input |= NetworkInput.MEDIUM_BYTE;
                NetworkInput.TWO_MEDIUM_INPUT = false;
            }
            if (NetworkInput.TWO_HEAVY_INPUT)
            {
                input |= NetworkInput.HEAVY_BYTE;
                NetworkInput.TWO_HEAVY_INPUT = false;
            }
            if (NetworkInput.TWO_ARCANA_INPUT)
            {
                input |= NetworkInput.ARCANA_BYTE;
                NetworkInput.TWO_ARCANA_INPUT = false;
            }
            if (NetworkInput.TWO_SHADOW_INPUT)
            {
                input |= NetworkInput.SHADOW_BYTE;
                NetworkInput.TWO_SHADOW_INPUT = false;
            }
            if (NetworkInput.TWO_GRAB_INPUT)
            {
                input |= NetworkInput.GRAB_BYTE;
                NetworkInput.TWO_GRAB_INPUT = false;
            }
            if (NetworkInput.TWO_BLUE_FRENZY_INPUT)
            {
                input |= NetworkInput.BLUE_FRENZY_BYTE;
                NetworkInput.TWO_BLUE_FRENZY_INPUT = false;
            }
            if (NetworkInput.TWO_RED_FRENZY_INPUT)
            {
                input |= NetworkInput.RED_FRENZY_BYTE;
                NetworkInput.TWO_RED_FRENZY_INPUT = false;
            }
            if (NetworkInput.TWO_DASH_FORWARD_INPUT)
            {
                input |= NetworkInput.DASH_FORWARD_BYTE;
                NetworkInput.TWO_DASH_FORWARD_INPUT = false;
            }
            if (NetworkInput.TWO_DASH_BACKWARD_INPUT)
            {
                input |= NetworkInput.DASH_BACKWARD_BYTE;
                NetworkInput.TWO_DASH_BACKWARD_INPUT = false;
            }
        }
        return input;
    }

    public void FreeBytes(NativeArray<byte> data)
    {
        if (data.IsCreated)
        {
            data.Dispose();
        }
    }

    public override int GetHashCode()
    {
        int hashCode = -1214587014;
        hashCode = hashCode * -1521134295 + Framenumber.GetHashCode();
        foreach (var player in _players)
        {
            hashCode = hashCode * -1521134295 + player.GetHashCode();
        }
        return hashCode;
    }

    public void LogInfo(string filename)
    {
        //Log
    }
    private bool Collision(PlayerNetwork player, PlayerNetwork otherPlayer)
    {
        if (Colliding(player, otherPlayer))
        {
            if (player.position.y > otherPlayer.position.y)
            {
                if (player.velocity.y < otherPlayer.velocity.y)
                {
                    float difference = Mathf.Abs(player.position.x - otherPlayer.position.x);
                    float pushDistance = (1.35f - difference) / (2);
                    if (player.position.x <= (float)DemonicsPhysics.WALL_LEFT_POINT)
                    {
                        player.position = new Vector2(player.position.x + pushDistance, player.position.y);
                    }
                    else if (player.position.x >= (float)DemonicsPhysics.WALL_RIGHT_POINT)
                    {
                        player.position = new Vector2(player.position.x - pushDistance, player.position.y);
                    }
                }
            }
            Vector2 main = player.velocity;
            Vector2 second = otherPlayer.velocity;
            if (otherPlayer.position.x >= (float)DemonicsPhysics.WALL_RIGHT_POINT && player.velocity.x >= 0 || otherPlayer.position.x <= (float)DemonicsPhysics.WALL_LEFT_POINT && player.velocity.x <= 0)
            {
                main = new Vector2(0, player.velocity.y);
                second = new Vector2(0, otherPlayer.velocity.y);
                otherPlayer.position = (new Vector2(otherPlayer.position.x + second.x, otherPlayer.position.y + second.y));
                player.position = (new Vector2(player.position.x + main.x, player.position.y + main.y));
                Intersects(player, otherPlayer);
                return true;
            }
            if (Mathf.Abs(player.velocity.x) > Mathf.Abs(otherPlayer.velocity.x))
            {
                float totalVelocity;
                if (player.velocity.x > 0 && otherPlayer.velocity.x < 0)
                {
                    totalVelocity = Mathf.Abs(player.velocity.x) - Mathf.Abs(otherPlayer.velocity.x);
                }
                else
                {
                    totalVelocity = Mathf.Abs(player.velocity.x);
                }
                if (player.position.x < otherPlayer.position.x && player.velocity.x > 0)
                {
                    main = new Vector2(totalVelocity, player.velocity.y);
                    second = new Vector2(totalVelocity, otherPlayer.velocity.y);
                    otherPlayer.position = (new Vector2(otherPlayer.position.x + second.x, otherPlayer.position.y + second.y));
                    player.position = (new Vector2(player.position.x + main.x, player.position.y + main.y));
                    Intersects(player, otherPlayer);
                    return true;
                }
                else if (player.position.x > otherPlayer.position.x && player.velocity.x < 0)
                {
                    main = new Vector2(-totalVelocity, player.velocity.y);
                    second = new Vector2(-totalVelocity, otherPlayer.velocity.y);
                    otherPlayer.position = (new Vector2(otherPlayer.position.x + second.x, otherPlayer.position.y + second.y));
                    player.position = (new Vector2(player.position.x + main.x, player.position.y + main.y));
                    Intersects(player, otherPlayer);
                    return true;
                }
                if (player.velocity.x == 0 || otherPlayer.velocity.x == 0)
                {
                    Intersects(player, otherPlayer);
                }
                return false;
            }
            else if (Mathf.Abs(player.velocity.x) == Mathf.Abs(otherPlayer.velocity.x))
            {
                if (player.position.x < otherPlayer.position.x && player.velocity.x > 0 || player.position.x > otherPlayer.position.x && player.velocity.x < 0)
                {
                    main = new Vector2(0, player.velocity.y);
                    second = new Vector2(0, otherPlayer.velocity.y);
                    player.position = (new Vector2(player.position.x + main.x, player.position.y + main.y));
                    Intersects(player, otherPlayer);
                    return true;
                }
                Intersects(player, otherPlayer);
                return false;
            }
            return true;
        }
        return false;
    }
    private void Intersects(PlayerNetwork player, PlayerNetwork otherPlayer)
    {
        if (player.position.x <= (float)DemonicsPhysics.WALL_LEFT_POINT || player.position.x >= (float)DemonicsPhysics.WALL_RIGHT_POINT)
        {
            return;
        }
        if (player.position.x < otherPlayer.position.x)
        {
            if (player.position.x + 1.35f >= otherPlayer.position.x)
            {
                float difference = Mathf.Abs(player.position.x - otherPlayer.position.x);
                float pushDistance = (1.35f - difference) / (2);
                player.position = new Vector2((player.position.x - pushDistance), player.position.y);
            }
        }
        else
        {
            if (player.position.x <= otherPlayer.position.x + 1.35f)
            {
                float difference = Mathf.Abs(player.position.x - otherPlayer.position.x);
                float pushDistance = (1.35f - difference) / (2);
                player.position = new Vector2((player.position.x + pushDistance), player.position.y);
            }
        }
    }
    private bool valueInRange(float value, float min, float max)
    { return (value >= min) && (value <= max); }
    private bool Colliding(PlayerNetwork a, PlayerNetwork b)
    {
        bool xOverlap = valueInRange(a.position.x - (1.35f / 2), b.position.x - (1.35f / 2), b.position.x + (1.35f / 2)) ||
                    valueInRange(b.position.x - (1.35f / 2), a.position.x - (1.35f / 2), a.position.x + (1.35f / 2));
        bool yOverlap = valueInRange(a.position.y - (1.5f / 2), b.position.y - (1.5f / 2), b.position.y + (1.5f / 2)) ||
                    valueInRange(b.position.y - (1.5f / 2), a.position.y - (1.5f / 2), a.position.y + (1.5f / 2));
        return xOverlap && yOverlap;
    }
}
[Serializable]
public class States
{
    public float Gravity;
    public virtual void Enter(PlayerNetwork player)
    {
        player.animationFrames = 0;
        Gravity = 0.018f;
    }
    public virtual void UpdateLogic(PlayerNetwork player) { }
    public virtual void Exit() { }
    public virtual bool ToAttackState() { return false; }
    public virtual bool ToDashState(PlayerNetwork player) { return false; }
};
[Serializable]
public class GroundParentStates : States
{
    public override void UpdateLogic(PlayerNetwork player)
    {
        base.UpdateLogic(player);
        player.canDoubleJump = true;
        player.canDash = true;
        player.hasJumped = false;
        player.canJump = true;
    }
    //TODO
}
[Serializable]
public class IdleStates : GroundParentStates
{
    public override void Enter(PlayerNetwork player)
    {
    }

    public override void UpdateLogic(PlayerNetwork player)
    {
        player.canDash = true;
        player.canDoubleJump = true;
        player.animation = "Idle";
        player.animationFrames++;
        player.velocity = new Vector2(0, 0);
        player.position = new Vector2(player.position.x + player.velocity.x, (float)DemonicsPhysics.GROUND_POINT);
        ToWalkState(player);
        ToJumpState(player);
        ToJumpForwardState(player);
        ToCrouchState(player);
    }
    private void ToCrouchState(PlayerNetwork player)
    {
        if (player.direction.y < 0)
        {
            player.state = "Crouch";
        }
    }
    private void ToJumpState(PlayerNetwork player)
    {
        if (player.direction.y > 0)
        {
            player.state = "Jump";
        }
    }
    private void ToJumpForwardState(PlayerNetwork player)
    {
        if (player.direction.y > 0 && player.direction.x != 0)
        {
            player.state = "JumpForward";
        }
    }
    private void ToWalkState(PlayerNetwork player)
    {
        if (player.direction.x != 0)
        {
            player.state = "Walk";
        }
    }
    public override bool ToDashState(PlayerNetwork player)
    {
        if (player.canDash)
        {
            player.state = "Dash";
            return true;
        }
        return false;
    }
}
public class CrouchStates : GroundParentStates
{
    public override void Enter(PlayerNetwork player)
    {

    }

    public override void UpdateLogic(PlayerNetwork player)
    {
        player.canDoubleJump = true;
        player.dashFrames = 0;
        player.animationFrames = 0;
        player.animation = "Crouch";
        player.velocity = new Vector2(0, 0);
        player.position = new Vector2(player.position.x + player.velocity.x, (float)DemonicsPhysics.GROUND_POINT);
        ToIdleState(player);
    }

    private void ToIdleState(PlayerNetwork player)
    {
        if (player.direction.y >= 0)
        {
            player.state = "Idle";
        }
    }
}
public class WalkStates : GroundParentStates
{
    public override void Enter(PlayerNetwork player)
    {

    }

    public override void UpdateLogic(PlayerNetwork player)
    {
        player.canDoubleJump = true;
        player.animation = "Walk";
        player.animationFrames++;
        player.velocity = new Vector2(player.direction.x * (float)player.playerStats.SpeedWalk, 0);
        player.position = new Vector2(player.position.x + player.velocity.x, (float)DemonicsPhysics.GROUND_POINT);
        ToIdleState(player);
        ToJumpState(player);
        ToJumpForwardState(player);
        ToCrouchState(player);
    }
    private void ToCrouchState(PlayerNetwork player)
    {
        if (player.direction.y < 0)
        {
            player.state = "Crouch";
        }
    }
    private void ToJumpState(PlayerNetwork player)
    {
        if (player.direction.y > 0)
        {
            player.state = "Jump";
        }
    }
    private void ToJumpForwardState(PlayerNetwork player)
    {
        if (player.direction.y > 0 && player.direction.x != 0)
        {
            player.state = "JumpForward";
        }
    }
    private void ToIdleState(PlayerNetwork player)
    {
        if (player.direction.x == 0)
        {
            player.state = "Idle";
        }
    }
    public override bool ToDashState(PlayerNetwork player)
    {
        if (player.canDash)
        {
            player.state = "Dash";
            return true;
        }
        return false;
    }
}
public class RunStates : GroundParentStates
{
    public override void Enter(PlayerNetwork player)
    {

    }

    public override void UpdateLogic(PlayerNetwork player)
    {
        player.animation = "Run";
        player.animationFrames++;
        player.velocity = new Vector2(player.direction.x * (float)player.playerStats.SpeedRun, 0);
        player.position = new Vector2(player.position.x + player.velocity.x, (float)DemonicsPhysics.GROUND_POINT);
        if (DemonicsWorld.Frame % 5 == 0)
        {
            if (player.flip > 0)
            {
                Vector2 effectPosition = new Vector2(player.position.x - 1, player.position.y);
                player.SetEffect("Ghost", player.position, false);
            }
            else
            {
                Vector2 effectPosition = new Vector2(player.position.x + 1, player.position.y);
                player.SetEffect("Ghost", player.position, true);
            }
        }
        ToIdleState(player);
    }

    private void ToIdleState(PlayerNetwork player)
    {
        if (player.direction.x == 0)
        {
            player.soundStop = "Run";
            player.state = "Idle";
        }
    }
}
[Serializable]
public class DashStates : States
{
    public override void Enter(PlayerNetwork player)
    {
    }

    public override void UpdateLogic(PlayerNetwork player)
    {
        if (player.skip == false)
        {
            player.sound = "Dash";
            if (player.dashDirection > 0)
            {
                Vector2 effectPosition = new Vector2(player.position.x - 1, player.position.y);
                player.SetEffect("Dash", effectPosition, false);
            }
            else
            {
                Vector2 effectPosition = new Vector2(player.position.x + 1, player.position.y);
                player.SetEffect("Dash", effectPosition, true);
            }
            player.dashFrames = 15;
            player.skip = true;
            player.velocity = new Vector2(player.dashDirection * (float)player.playerStats.DashForce, 0);
        }
        Dash(player);
    }

    private void Dash(PlayerNetwork player)
    {
        if (player.dashFrames > 0)
        {
            player.animationFrames = 0;
            player.animation = "Dash";
            if (player.dashFrames % 5 == 0)
            {
                if (player.flip > 0)
                {
                    Vector2 effectPosition = new Vector2(player.position.x - 1, player.position.y);
                    player.SetEffect("Ghost", player.position, false);
                }
                else
                {
                    Vector2 effectPosition = new Vector2(player.position.x + 1, player.position.y);
                    player.SetEffect("Ghost", player.position, true);
                }
            }
            player.position = new Vector2(player.position.x + player.velocity.x, (float)DemonicsPhysics.GROUND_POINT);
            player.dashFrames--;
        }
        else
        {
            player.velocity = Vector2.zero;
            player.skip = false;
            if (player.direction.x * player.flip > 0)
            {
                player.sound = "Run";
                player.state = "Run";
            }
            else
            {
                player.state = "Idle";
            }
        }
    }
}

public class DashAirState : States
{
    public override void Enter(PlayerNetwork player)
    {
    }

    public override void UpdateLogic(PlayerNetwork player)
    {
        if (player.skip == false)
        {
            player.sound = "Dash";
            if (player.dashDirection > 0)
            {
                Vector2 effectPosition = new Vector2(player.position.x - 1, player.position.y);
                player.SetEffect("Dash", effectPosition, false);
            }
            else
            {
                Vector2 effectPosition = new Vector2(player.position.x + 1, player.position.y);
                player.SetEffect("Dash", effectPosition, true);
            }
            player.canDash = false;
            player.dashFrames = 15;
            player.skip = true;
            player.velocity = new Vector2(player.dashDirection * (float)player.playerStats.DashForce, 0);
        }
        Dash(player);
    }

    private void Dash(PlayerNetwork player)
    {
        if (player.dashFrames > 0)
        {
            player.animationFrames = 0;
            player.animation = "Fall";
            if (player.dashFrames % 5 == 0)
            {
                if (player.flip > 0)
                {
                    Vector2 effectPosition = new Vector2(player.position.x - 1, player.position.y);
                    player.SetEffect("Ghost", player.position, false);
                }
                else
                {
                    Vector2 effectPosition = new Vector2(player.position.x + 1, player.position.y);
                    player.SetEffect("Ghost", player.position, true);
                }
            }
            player.position = new Vector2(player.position.x + player.velocity.x, player.position.y);
            player.dashFrames--;
        }
        else
        {
            player.velocity = Vector2.zero;
            player.skip = false;
            player.state = "Fall";
        }
    }
}



public class AirParentStates : States
{
    public override void Enter(PlayerNetwork player)
    {
    }

    public override void UpdateLogic(PlayerNetwork player)
    {
        ToJumpForwardState(player);
        ToJumpState(player);
        //ToFallState(player);
    }
    private void ToJumpState(PlayerNetwork player)
    {
        if (player.canDoubleJump)
        {
            if (player.direction.y > 0 && !player.hasJumped)
            {
                player.skip = false;
                player.hasJumped = true;
                player.canDoubleJump = false;
                player.state = "Jump";
            }
            else if (player.direction.y <= 0 && player.hasJumped)
            {
                player.hasJumped = false;
            }
        }
    }
    private void ToJumpForwardState(PlayerNetwork player)
    {
        if (player.canDoubleJump)
        {
            if (player.direction.y > 0 && player.direction.x != 0 && !player.hasJumped)
            {
                player.skip = false;
                player.hasJumped = true;
                player.canDoubleJump = false;
                player.state = "JumpForward";
            }
            else if (player.direction.y <= 0 && player.hasJumped)
            {
                player.hasJumped = false;
            }
        }
    }
    private void ToFallState(PlayerNetwork player)
    {
        if (player.velocity.y <= 0)
        {
            player.skip = false;
            player.state = "Fall";
        }
    }
    public override bool ToDashState(PlayerNetwork player)
    {
        if (player.canDash)
        {
            player.skip = false;
            player.velocity = Vector2.zero;
            player.state = "DashAir";
            return true;
        }
        return false;
    }
}
public class JumpStates : AirParentStates
{
    public override void Enter(PlayerNetwork player)
    {

    }

    public override void UpdateLogic(PlayerNetwork player)
    {
        if (player.skip == false)
        {
            player.sound = "Jump";
            player.SetEffect("Jump", player.position);
            player.hasJumped = true;
            player.animationFrames = 0;
            player.skip = true;
            player.velocity = new Vector2(0, (float)player.playerStats.JumpForce);
        }
        player.animation = "Jump";
        player.animationFrames++;
        player.velocity = new Vector2(player.velocity.x, player.velocity.y - player.gravity);
        player.position = new Vector2(player.position.x, player.position.y + player.velocity.y);
        base.UpdateLogic(player);
        ToFallState(player);
    }
    private void ToFallState(PlayerNetwork player)
    {
        if (player.velocity.y <= 0)
        {
            player.skip = false;
            player.state = "Fall";
        }
    }
}

public class JumpForwardStates : AirParentStates
{
    public override void UpdateLogic(PlayerNetwork player)
    {
        if (player.skip == false)
        {
            player.sound = "Jump";
            player.SetEffect("Jump", player.position);
            player.hasJumped = true;
            player.animationFrames = 0;
            player.skip = true;
            player.velocity = new Vector2(0.14f * player.direction.x, (float)player.playerStats.JumpForce);
        }
        player.animation = "JumpForward";
        player.animationFrames++;
        player.velocity = new Vector2(player.velocity.x, player.velocity.y - player.gravity);
        player.position = new Vector2(player.position.x + player.velocity.x, player.position.y + player.velocity.y);
        base.UpdateLogic(player);
        ToFallState(player);
    }
    private void ToFallState(PlayerNetwork player)
    {
        if (player.velocity.y <= 0)
        {
            player.skip = false;
            player.state = "Fall";
        }
    }
}
public class FallStates : AirParentStates
{
    public override void Enter(PlayerNetwork player)
    {
    }

    public override void UpdateLogic(PlayerNetwork player)
    {
        player.animationFrames = 0;
        player.animation = "Fall";
        player.velocity = new Vector2(player.velocity.x, player.velocity.y - player.gravity);
        player.position = new Vector2(player.position.x + player.velocity.x, player.position.y + player.velocity.y);
        base.UpdateLogic(player);
        ToIdleState(player);
    }
    private void ToIdleState(PlayerNetwork player)
    {
        if ((DemonicsFloat)player.position.y <= DemonicsPhysics.GROUND_POINT && (DemonicsFloat)player.velocity.y <= (DemonicsFloat)0)
        {
            player.sound = "Landed";
            player.SetEffect("Fall", player.position);
            player.state = "Idle";
        }
    }
}
public class AttackStates : States
{
    public override void Enter(PlayerNetwork player)
    {
        base.Enter(player);
        player.animation = "5M";
    }

    public override void UpdateLogic(PlayerNetwork player)
    {
        base.UpdateLogic(player);
        ToIdleState(player);
        player.animationFrames++;
    }

    private void ToIdleState(PlayerNetwork player)
    {

    }
}