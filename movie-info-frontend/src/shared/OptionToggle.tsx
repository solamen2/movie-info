import "./shared.css";

interface OptionToggleProps {
  // The group's accessible name, e.g. "plot source"
  label: string;
  options: string[];
  selected: string;
  onSelect: (option: string) => void;
}

// A row of buttons that switches a section between several things to show
// (a plot's sources, the ways to watch a title), with the one shown marked.
function OptionToggle({
  label,
  options,
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
          {option}
        </button>
      ))}
    </div>
  );
}

export default OptionToggle;
