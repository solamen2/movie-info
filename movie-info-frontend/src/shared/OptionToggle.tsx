import "./shared.css";

interface OptionToggleProps {
  // The group's accessible name, e.g. "plot source"
  label: string;
  options: string[];
  // What an option's button says, when that is more than the option itself
  // (e.g. with a count: "Director (5)")
  labelOf?: (option: string) => string;
  selected: string;
  onSelect: (option: string) => void;
}

// A row of buttons that switches a section between several things to show
// (a plot's sources, the ways to watch a title), with the one shown marked.
function OptionToggle({
  label,
  options,
  labelOf,
  selected,
  onSelect,
}: OptionToggleProps) {
  return (
    <div className="option-toggle" role="group" aria-label={label}>
      {options.map((option) => (
        <button
          key={option}
          type="button"
          aria-pressed={option === selected}
          onClick={() => {
            onSelect(option);
          }}
        >
          {labelOf ? labelOf(option) : option}
        </button>
      ))}
    </div>
  );
}

export default OptionToggle;
