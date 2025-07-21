// IInteractable.cs
using UnityEngine;

/// <summary>
/// Defines an object that the player can interact with.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Checks if the object is currently available for interaction.
    /// </summary>
    /// <returns>True if the object can be interacted with, false otherwise.</returns>
    bool IsInteractable();

    /// <summary>
    /// Called by the player to perform an interaction.
    /// </summary>
    /// <param name="player">The PlayerController instance that is initiating the interaction.</param>
    void Interact(PlayerController player);
}