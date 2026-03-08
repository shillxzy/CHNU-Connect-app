export const Group = ({ groups, joiningGroupId, handleJoinGroup }) => {
  return (
    <>
      {groups.length > 0 ? (
        groups.map((group) => (
          <div className="group-card" key={group.id}>
            <div className="group-info">
              <div className="group-field">
                <strong>Назва:</strong> {group.name}
              </div>
              <div className="group-field">
                <strong>Опис:</strong> {group.description}
              </div>
              <div className="group-field">
                <strong>Учасники:</strong> {group.membersCount}
              </div>
            </div>
            <button
              className="btn-join"
              disabled={joiningGroupId === group.id}
              onClick={() => handleJoinGroup(group.id)}
            >
              {joiningGroupId === group.id ? "Приєднання..." : "Приєднатися"}
            </button>
          </div>
        ))
      ) : (
        <p>Немає груп</p>
      )}
    </>
  );
};