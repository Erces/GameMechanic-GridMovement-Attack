using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField]
    private EntityData entityData;

    private int actionPoints;
    private int attack;
    private int range;
    private int health;
    private int armor;
    private int speed;

    private void Awake()
    {
        ResetAllPrefs();
        ResetActionPoints();
    }

    public enum SetType
    {
        INCREASE,
        DECREASE
    }

    public enum PrefType
    {
        ATTACK,
        RANGE,
        HEALTH,
        ARMOR,
        SPEED
    }

    public void SetPref(int amount, SetType setType ,PrefType prefType)
    {
        switch (prefType)
        {
            case PrefType.ATTACK:
                if (setType == SetType.INCREASE) attack += amount;
                else if (setType == SetType.DECREASE) attack -= amount;
                break;
            case PrefType.RANGE:
                if (setType == SetType.INCREASE) range += amount;
                else if (setType == SetType.DECREASE) range -= amount;
                break;
            case PrefType.HEALTH:
                if (setType == SetType.INCREASE) health += amount;
                else if (setType == SetType.DECREASE) health -= amount;
                break;
            case PrefType.ARMOR:
                if (setType == SetType.INCREASE) armor += amount;
                else if (setType == SetType.DECREASE) armor -= amount;
                break;
            case PrefType.SPEED:
                if (setType == SetType.INCREASE) speed += amount;
                else if (setType == SetType.DECREASE) speed -= amount;
                break;
        }
    }

    public void GetAllPrefs(out int attack, out int range, out int health, out int armor, out int speed)
    {
        attack = this.attack;
        range = this.range;
        health = this.health;
        armor = this.armor;
        speed = this.speed;
    }
    public void ResetAllPrefs()
    {
        attack = entityData.attack;
        range = entityData.range;
        health = entityData.health;
        armor = entityData.armor;
        speed = entityData.speed;
    }

    public int GetAttack() => attack;
    public void SetAttack(int attack) { this.attack = attack; }

    public int GetRange() => range;
    public void SetRange(int range) { this.range = range; }

    public int GetHealth() => health;
    public void SetHealth(int health) { this.health = health; }

    public int GetArmor() => armor;
    public void SetArmor(int armor) { this.armor = armor; }

    public int GetSpeed() => speed;
    public void SetSpeed(int speed) { this.speed = speed; }

    public int GetActionPoints() => actionPoints;
    public void DecreaseActionPoints(int amount) { actionPoints -= amount; }
    public void IncreaseActionPoints(int amount) { actionPoints += amount; }
    public void SetActionPoints(int actionPoints) { this.actionPoints = actionPoints; }
    public void ResetActionPoints() { actionPoints = entityData.actionPoints; }
}
