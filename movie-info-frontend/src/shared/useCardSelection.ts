import { useEffect, useState } from "react";

// Keep in sync with the `select-fly` / `deselect-fly` animation duration and
// the `.expandable-card` width transition duration in shared.css.
export const CARD_FLY_MS = 500;
export const CARD_RESIZE_MS = 300;

// Delayed second half of a two-phase card animation: expand the selected card
// into its panel once it has flown to the corner, or fly the card home once
// its panel has shrunk back to card size.
type PendingPhase = "expand" | "deselect";

// Which card in a list is selected and where it is in its animation. Always
// replaced as a whole via the helpers below, since the fields only make sense
// together.
export interface CardSelection {
  selectedId: string | null;
  // The card the user just transitioned out of selected. It gets a
  // `.deselecting` class which fires the `deselect-fly` keyframe animation in
  // CSS — required because CSS animations only run on class addition, not on
  // class removal, so we can't rely on .selected alone to animate both
  // directions.
  previouslySelectedId: string | null;
  // A selected card with a detail panel animates in two phases: first it flies
  // to the upper-left (selectedId), then it grows into the panel (expandedId).
  // Going back runs the phases in reverse.
  expandedId: string | null;
  // A fresh object per transition so the timer effect restarts even when two
  // transitions in a row wait on the same phase.
  pendingPhase: { kind: PendingPhase } | null;
}

const NO_SELECTION: CardSelection = {
  selectedId: null,
  previouslySelectedId: null,
  expandedId: null,
  pendingPhase: null,
};

// Shown already in its final state, with no animation (e.g. a directly loaded
// URL).
function atOnce(id: string | null, opensPanel: boolean): CardSelection {
  return {
    ...NO_SELECTION,
    selectedId: id,
    expandedId: opensPanel ? id : null,
  };
}

// The card flies to the corner and, if it opens a panel, expands afterwards.
function selectCard(
  prev: CardSelection,
  id: string,
  opensPanel: boolean,
): CardSelection {
  return {
    selectedId: id,
    previouslySelectedId: prev.selectedId,
    expandedId: null,
    pendingPhase: opensPanel ? { kind: "expand" } : null,
  };
}

// Switching straight from one expanded card to another (e.g. a person's panel
// opened by clicking a cast card inside a movie's panel): the new card is shown
// already expanded, with no flight, and the old card is dropped at once rather
// than flying home. The old card is still handed over as previouslySelectedId;
// its `.deselecting` class inside a container that still `.has-selection` is
// what tells the CSS to drop it (see shared.css).
function replaceCard(
  prev: CardSelection,
  id: string,
  opensPanel: boolean,
): CardSelection {
  return { ...atOnce(id, opensPanel), previouslySelectedId: prev.selectedId };
}

// The card flies back home.
function deselectCard(prev: CardSelection): CardSelection {
  return { ...NO_SELECTION, previouslySelectedId: prev.selectedId };
}

// The panel shrinks back to card size, after which the card is deselected.
function shrinkPanel(prev: CardSelection): CardSelection {
  return { ...prev, expandedId: null, pendingPhase: { kind: "deselect" } };
}

function expandCard(prev: CardSelection): CardSelection {
  return { ...prev, expandedId: prev.selectedId, pendingPhase: null };
}

interface CardSelectionOptions {
  // The card the URL says is selected, or null for none.
  targetId: string | null;
  // Whether the selected card grows into a detail panel after flying to the
  // corner, or merely stays highlighted.
  opensPanel: boolean;
  // Whether the list currently has a card for the id. A selected card that is
  // gone (or never existed) has no home to fly back to, so it is dropped at
  // once instead of animating.
  hasCardFor: (id: string) => boolean;
  // When this changes, the list's contents are about to be replaced, so the
  // selection jumps straight to the target state instead of animating.
  resetKey?: unknown;
}

// Drives the select / expand / shrink / deselect animation of a card list from
// a URL-derived target. The URL is the source of truth: clicking a card only
// navigates, and this hook animates towards whatever the URL now says, so
// browser back / forward and directly loaded URLs behave exactly like clicks.
export function useCardSelection({
  targetId,
  opensPanel,
  hasCardFor,
  resetKey,
}: CardSelectionOptions): CardSelection {
  const [selection, setSelection] = useState(() =>
    atOnce(targetId, opensPanel),
  );
  // The values the selection was last synced to.
  const [synced, setSynced] = useState({ resetKey, targetId });

  // Sync to the URL during render (see "Adjusting some state when a prop
  // changes" in the React docs), so the very next paint already reflects it.
  if (resetKey !== synced.resetKey) {
    setSynced({ resetKey, targetId });
    setSelection(atOnce(targetId, opensPanel));
  } else if (targetId !== synced.targetId) {
    setSynced({ resetKey, targetId });
    if (targetId) {
      setSelection(
        selection.expandedId
          ? replaceCard(selection, targetId, opensPanel)
          : selectCard(selection, targetId, opensPanel),
      );
    } else if (
      selection.expandedId &&
      selection.selectedId &&
      hasCardFor(selection.selectedId)
    ) {
      setSelection(shrinkPanel(selection));
    } else {
      setSelection(deselectCard(selection));
    }
  }

  const pendingPhase = selection.pendingPhase;
  useEffect(() => {
    if (!pendingPhase) return;
    const isExpand = pendingPhase.kind === "expand";
    const timeoutId = window.setTimeout(
      () => {
        setSelection(isExpand ? expandCard : deselectCard);
      },
      isExpand ? CARD_FLY_MS : CARD_RESIZE_MS,
    );
    return () => {
      window.clearTimeout(timeoutId);
    };
  }, [pendingPhase]);

  return selection;
}
