export function VehicleTable(props){
    return (
        
        <table class="table table-striped table-bordered">
            <thead>
                <tr>
                    <th scope="col">VRM</th>
                    <th scope="col">Colour</th>
                    <th scope="col">Make</th>
                </tr>
            </thead>
            <tbody>
                {props.vehicles.map(v => (
                    <tr scope="row" key={v.registrationNumber} class={ v.isError ? "table-danger" : ""}>
                        <td>{v.registrationNumber}</td>
                        {v.isError
                            ? <td colspan="2">{v.errorMessage}</td>
                            : <>
                            <td>{v.colour}</td>
                            <td>{v.make}</td>
                            </>}
                    </tr>
                    ))}
            </tbody>
            </table>
            
        );
}