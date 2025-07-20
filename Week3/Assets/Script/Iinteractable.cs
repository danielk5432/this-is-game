using UnityEngine;

/// <summary>
/// Defines an object that the player can interact with.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Checks if the object is currently available for interaction.
    /// </summary>
    /// <returns>True if interactable, false otherwise.</returns>
    bool IsInteractable();

    /// <summary>
    /// Called when the player interacts with this object.
    /// </summary>
    /// <param name="heldBox">The box data the player is holding. Can be null.</param>
    /// <returns>The data of a box given to the player. Null if no item was given.</returns>
    BoxData OnInteract(BoxData heldBox);

    // (Optional) A way for other scripts to get a reference to the GameObject.
    GameObject GetGameObject(); 
}